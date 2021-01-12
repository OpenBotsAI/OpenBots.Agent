using Autofac;
using OpenBots.Core.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using OpenBots.Core.Settings;

namespace OpenBots.Executor.Utilities
{
    public static class AssembliesManager
    {
        public static ContainerBuilder LoadBuilder(List<string> assemblyPaths)
        {
            List<Assembly> existingAssemblies = new List<Assembly>();
            foreach (var path in assemblyPaths)
            {
                try
                {
                    var name = AssemblyName.GetAssemblyName(path).Name;

                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var existingAssembly = assemblies.Where(x => x.GetName().Name == name &&
                                                                 x.GetName().Version.ToString() == AssemblyName.GetAssemblyName(path).Version.ToString())
                                                     .FirstOrDefault();

                    if (existingAssembly == null && name != "RestSharp" && name != "WebDriver" && 
                        name != "OpenBots.Core" && name != "Serilog.Sinks.File")
                    {
                        var assembly = Assembly.LoadFrom(path);
                        existingAssemblies.Add(assembly);
                    }
                    else if (existingAssembly != null)
                        existingAssemblies.Add(existingAssembly);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(existingAssemblies.ToArray())
                                                   .Where(t => t.IsAssignableTo<ScriptCommand>())
                                                   .Named<ScriptCommand>(t => t.Name)
                                                   .AsImplementedInterfaces();
            return builder;
        }
    }
}
