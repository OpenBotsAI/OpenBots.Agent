using Autofac;
using Newtonsoft.Json;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using OpenBots.Agent.Core.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenBots.Agent.Core.Nuget
{
    public static class NugetPackageManager
    {
        public static async Task GetPackageDependencies(PackageIdentity package,
                NuGetFramework framework,
                SourceCacheContext cacheContext,
                ILogger logger,
                IEnumerable<SourceRepository> repositories,
                ISet<SourcePackageDependencyInfo> availablePackages)
        {
            if (availablePackages.Contains(package)) return;

            foreach (var sourceRepository in repositories)
            {
                var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(
                    package, framework, cacheContext, logger, CancellationToken.None);

                if (dependencyInfo == null) continue;

                availablePackages.Add(dependencyInfo);
                foreach (var dependency in dependencyInfo.Dependencies)
                {
                    await GetPackageDependencies(
                        new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
                        framework, cacheContext, logger, repositories, availablePackages);
                }
            }
        }

        public static List<string> LoadPackageAssemblies(string configPath)
        {
            List<string> assemblyPaths = new List<string>();
            List<string> exceptionsList = new List<string>();
            var dependencies = JsonConvert.DeserializeObject<Project.Project>(File.ReadAllText(configPath)).Dependencies;

            string appDataPath = new EnvironmentSettings().GetEnvironmentVariable();
            string packagePath = Path.Combine(Directory.GetParent(appDataPath).Parent.FullName, "packages");
            var packagePathResolver = new PackagePathResolver(packagePath);

            var nuGetFramework = NuGetFramework.ParseFolder("net48");
            var settings = NuGet.Configuration.Settings.LoadDefaultSettings(root: null);

            var sourceRepositoryProvider = new SourceRepositoryProvider(new PackageSourceProvider(settings), Repository.Provider.GetCoreV3());
            var localRepo = sourceRepositoryProvider.CreateRepository(new PackageSource(packagePath, "Local OpenBots Repo", true));

            var resolver = new PackageResolver();
            var frameworkReducer = new FrameworkReducer();
            var repositories = new List<SourceRepository>
            {
                localRepo
            };

            Parallel.ForEach(dependencies, async dependency =>
            {
                try
                {
                    using (var cacheContext = new SourceCacheContext())
                    {
                        var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
                        await GetPackageDependencies(
                            new PackageIdentity(dependency.Key, NuGetVersion.Parse(dependency.Value)),
                            nuGetFramework, cacheContext, NullLogger.Instance, repositories, availablePackages);

                        var resolverContext = new PackageResolverContext(
                            DependencyBehavior.Lowest,
                            new[] { dependency.Key },
                            Enumerable.Empty<string>(),
                            Enumerable.Empty<PackageReference>(),
                            Enumerable.Empty<PackageIdentity>(),
                            availablePackages,
                            sourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
                            NullLogger.Instance);

                        var packagesToInstall = resolver.Resolve(resolverContext, CancellationToken.None)
                            .Select(p => availablePackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)));

                        foreach (var packageToInstall in packagesToInstall)
                        {
                            PackageReaderBase packageReader = new PackageFolderReader(packagePathResolver.GetInstalledPath(packageToInstall));

                            var nearest = frameworkReducer.GetNearest(nuGetFramework, packageReader.GetLibItems().Select(x => x.TargetFramework));

                            var packageListAssemblyPaths = packageReader.GetLibItems()
                                .Where(x => x.TargetFramework.Equals(nearest))
                                .SelectMany(x => x.Items.Where(i => i.EndsWith(".dll"))).ToList();

                            if (packageListAssemblyPaths != null)
                            {
                                foreach (string path in packageListAssemblyPaths)
                                {
                                    if (!assemblyPaths.Contains(Path.Combine(packagePath, $"{packageToInstall.Id}.{packageToInstall.Version}", path)))
                                        assemblyPaths.Add(Path.Combine(packagePath, $"{packageToInstall.Id}.{packageToInstall.Version}", path));
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    exceptionsList.Add($"Unable to load {packagePath}\\{dependency.Key}.{dependency.Value}");
                }
            });
            if(exceptionsList.Count>0)
            {
                exceptionsList.Add("Please install this package using the OpenBots Studio Package Manager");
                throw new Exception(string.Join("\n", exceptionsList));
            }
            return assemblyPaths;
        }
    }
}
