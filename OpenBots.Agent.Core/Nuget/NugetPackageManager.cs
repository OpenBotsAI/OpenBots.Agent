using Autofac;
using Newtonsoft.Json;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using OpenBots.Agent.Core.Model;
using OpenBots.Core.Enums;
using OpenBots.Core.IO;
using OpenBots.Core.Project;
using OpenBots.Core.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        public static List<string> LoadPackageAssemblies(string configPath, string userName)
        {
            List<string> assemblyPaths = new List<string>();
            List<string> exceptionsList = new List<string>();
            var dependencies = JsonConvert.DeserializeObject<Project>(File.ReadAllText(configPath)).Dependencies;

            string appDataPath = new EnvironmentSettings().GetEnvironmentVariablePath(userName);
            string packagePath = Path.Combine(Directory.GetParent(appDataPath).FullName, "packages");
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
            if (exceptionsList.Count > 0)
            {
                exceptionsList.Add("Please install this package using the OpenBots Studio Package Manager");
                throw new Exception(string.Join("\n", exceptionsList));
            }

            List<string> filteredPaths = new List<string>();
            foreach (string path in assemblyPaths)
            {
                if (filteredPaths.Where(a => a.Contains(path.Split('/').Last()) && FileVersionInfo.GetVersionInfo(a).FileVersion ==
                                        FileVersionInfo.GetVersionInfo(path).FileVersion).FirstOrDefault() == null)
                    filteredPaths.Add(path);
            }

            return filteredPaths;
        }
        public static async Task InstallPackage(string packageId, string version, Dictionary<string, string> projectDependenciesDict, string userName, string installDefaultSource = "")
        {
            string appSettingsDirPath = Directory.GetParent(new EnvironmentSettings().GetEnvironmentVariablePath(userName)).FullName;
            var appSettings = new ApplicationSettings().GetOrCreateApplicationSettings(appSettingsDirPath);
            var packageSources = appSettings.ClientSettings.PackageSourceDT.AsEnumerable()
                            .Where(r => r.Field<string>(0) == "True")
                            .CopyToDataTable();

            var packageVersion = NuGetVersion.Parse(version);
            var nuGetFramework = NuGetFramework.ParseFolder("net48");
            var settings = NuGet.Configuration.Settings.LoadDefaultSettings(root: null);
            var sourceRepositoryProvider = new SourceRepositoryProvider(new PackageSourceProvider(settings), Repository.Provider.GetCoreV3());

            using (var cacheContext = new SourceCacheContext())
            {
                var repositories = new List<SourceRepository>();
                if (!string.IsNullOrEmpty(installDefaultSource))
                {
                    var sourceRepo = sourceRepositoryProvider.CreateRepository(new PackageSource(installDefaultSource, "Default Packages Source", true));
                    repositories.Add(sourceRepo);
                }
                else
                {
                    foreach (DataRow row in packageSources.Rows)
                    {
                        var sourceRepo = sourceRepositoryProvider.CreateRepository(new PackageSource(row[2].ToString(), row[1].ToString(), true));
                        repositories.Add(sourceRepo);
                    }
                }

                var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
                await GetPackageDependencies(
                    new PackageIdentity(packageId, packageVersion),
                    nuGetFramework, cacheContext, NullLogger.Instance, repositories, availablePackages);

                var resolverContext = new PackageResolverContext(
                    DependencyBehavior.Lowest,
                    new[] { packageId },
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<PackageReference>(),
                    Enumerable.Empty<PackageIdentity>(),
                    availablePackages,
                    sourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
                    NullLogger.Instance);

                var resolver = new PackageResolver();
                var packagesToInstall = resolver.Resolve(resolverContext, CancellationToken.None)
                    .Select(p => availablePackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)));
                var packagePathResolver = new PackagePathResolver(Path.Combine(appSettingsDirPath, "packages"));
                var packageExtractionContext = new PackageExtractionContext(
                    PackageSaveMode.Defaultv3,
                    XmlDocFileSaveMode.None,
                    ClientPolicyContext.GetClientPolicy(settings, NullLogger.Instance),
                    NullLogger.Instance);

                var frameworkReducer = new FrameworkReducer();
                PackageReaderBase packageReader;
                PackageDownloadContext downloadContext = new PackageDownloadContext(cacheContext);

                foreach (var packageToInstall in packagesToInstall)
                {
                    var installedPath = packagePathResolver.GetInstalledPath(packageToInstall);
                    if (installedPath == null)
                    {
                        var downloadResource = await packageToInstall.Source.GetResourceAsync<DownloadResource>(CancellationToken.None);
                        var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                            packageToInstall,
                            downloadContext,
                            SettingsUtility.GetGlobalPackagesFolder(settings),
                            NullLogger.Instance, CancellationToken.None);

                        await PackageExtractor.ExtractPackageAsync(
                            downloadResult.PackageSource,
                            downloadResult.PackageStream,
                            packagePathResolver,
                            packageExtractionContext,
                            CancellationToken.None);

                        packageReader = downloadResult.PackageReader;
                    }
                    else
                        packageReader = new PackageFolderReader(installedPath);

                    if (packageToInstall.Id == packageId)
                    {
                        if (projectDependenciesDict.ContainsKey(packageToInstall.Id))
                            projectDependenciesDict[packageToInstall.Id] = packageToInstall.Version.ToString();
                        else
                            projectDependenciesDict.Add(packageToInstall.Id, packageToInstall.Version.ToString());
                    }
                }
            }
        }
        public static void InstallProjectDependencies(string configPath, string userName)
        {
            var dependencies = JsonConvert.DeserializeObject<Project>(File.ReadAllText(configPath)).Dependencies;
            string appDataPath = new EnvironmentSettings().GetEnvironmentVariablePath(userName);
            string packagesFolderPath = Path.Combine(Directory.GetParent(appDataPath).FullName, "packages");

            // Install Project Dependencies
            foreach (var dependency in dependencies)
            {
                if (!Directory.Exists(Path.Combine(packagesFolderPath, $"{dependency.Key}.{dependency.Value}")))
                {
                    try
                    {
                        Task.Run(async () => await InstallPackage(dependency.Key, dependency.Value, new Dictionary<string, string>(), userName)).GetAwaiter().GetResult();
                    }
                    catch (Exception excep)
                    {
                        throw excep;
                    }
                }
            }
        }

        public static void SetupFirstTimeUserEnvironment(string userName, string productVersion)
        {
            string packagesPath = Folders.GetFolder(FolderType.LocalAppDataPackagesFolder);
            string programPackagesSource = Folders.GetFolder(FolderType.ProgramFilesPackagesFolder);

            if (!Directory.Exists(programPackagesSource))
                throw new DirectoryNotFoundException($"Unable to find '{programPackagesSource}' during installation of commands packages.");

            var commandVersion = Regex.Matches(productVersion, @"\d+\.\d+\.\d+")[0].ToString();

            Dictionary<string, string> dependencies = Project.DefaultCommandGroups.ToDictionary(x => $"OpenBots.Commands.{x}", x => commandVersion);

            List<string> existingOpenBotsPackages = Directory.GetDirectories(packagesPath)
                                                             .Where(x => new DirectoryInfo(x).Name.StartsWith("OpenBots"))
                                                             .ToList();
            foreach (var dep in dependencies)
            {
                string existingDirectory = existingOpenBotsPackages.Where(x => new DirectoryInfo(x).Name.StartsWith(dep.Key))
                                                                   .FirstOrDefault();
                if (existingDirectory == null)
                {
                    Task.Run(async () => await InstallPackage(dep.Key, dep.Value, new Dictionary<string, string>(), userName, programPackagesSource)).GetAwaiter().GetResult();
                }
            }
        }
    }
}
