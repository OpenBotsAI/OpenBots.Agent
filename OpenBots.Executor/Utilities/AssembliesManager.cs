using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenBots.Executor.Utilities
{
    public static class AssembliesManager
    {
        public static List<Assembly> LoadAssemblies(List<string> assemblyPaths)
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

                    if (existingAssembly == null && name != "OpenBots.Engine" && name != "RestSharp" && name != "WebDriver")
                    {
                        var assembly = Assembly.LoadFrom(path);
                        existingAssemblies.Add(assembly);
                    }
                    else if (name != "OpenBots.Engine" && name != "RestSharp" && name != "WebDriver")
                        existingAssemblies.Add(existingAssembly);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            var engineAssemblyPath = assemblyPaths.Where(p => Regex.Matches(p, "OpenBots.Engine").Count > 1).FirstOrDefault();
            Assembly engineAssembly = Assembly.LoadFrom(engineAssemblyPath);
            existingAssemblies.Add(engineAssembly);
            return existingAssemblies;
        }
    }
}
