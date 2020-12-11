using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenBots.Agent.Core.Nuget
{
    public class AppDomainSetupManager
    {
        public static ContainerBuilder LoadBuilder(List<string> assemblyPaths)
        {
            List<Assembly> existingAssemblies = new List<Assembly>();
            foreach (var path in assemblyPaths)
            {
                try
                {
                    var assemblyinfo = AssemblyName.GetAssemblyName(path);
                    var name = assemblyinfo.Name;
                    var version = assemblyinfo.Version.ToString();

                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var existingAssembly = assemblies.Where(x => x.GetName().Name == name &&
                                                                 x.GetName().Version.ToString() == version)
                                                     .FirstOrDefault();

                    if (existingAssembly == null/* && name != "OpenBots.Engine" && name != "RestSharp" && name != "WebDriver"*/)
                    {
                        var assembly = Assembly.LoadFrom(path);
                        existingAssemblies.Add(assembly);
                    }
                    //else if (name != "OpenBots.Engine" && name != "RestSharp" && name != "WebDriver")
                    //    existingAssemblies.Add(existingAssembly);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(existingAssemblies.ToArray());
                                                   //.Where(t => t.IsAssignableTo())
                                                   //.Named<ScriptCommand>(t => t.Name)
                                                   //.AsImplementedInterfaces();
            return builder;
        }

        public static Type GetTypeByName(IContainer container, string typeName)
        {
            using (var scope = container.BeginLifetimeScope())
            {
                var types = scope.ComponentRegistry.Registrations
                            .Where(r => r.Activator.LimitType.Name.Equals(typeName))
                            .Select(r => r.Activator.LimitType);

                var types1 = scope.ComponentRegistry.Registrations
                            .Select(r => r.Activator.LimitType);

                var type = types.Where(x => x.Name == typeName).FirstOrDefault();

                return type;
            }
        }
    }
}
