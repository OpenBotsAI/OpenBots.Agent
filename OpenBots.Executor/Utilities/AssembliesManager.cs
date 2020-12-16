using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenBots.Executor.Utilities
{
    public static class AssembliesManager
    {
        public static List<Assembly> LoadAssemblies(List<string> assemblyPaths)
        {
            List<string> filteredPaths = new List<string>();
            foreach (string path in assemblyPaths)
            {
                if (filteredPaths.Where(a => a.Contains(path.Split('/').Last()) && FileVersionInfo.GetVersionInfo(a).FileVersion ==
                                        FileVersionInfo.GetVersionInfo(path).FileVersion).FirstOrDefault() == null)
                    filteredPaths.Add(path);
            }

            List<Assembly> existingAssemblies = new List<Assembly>();
            foreach (var path in filteredPaths)
            {
                try
                {
                    var name = AssemblyName.GetAssemblyName(path).Name;

                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var existingAssembly = assemblies.Where(x => x.GetName().Name == name &&
                                                                 x.GetName().Version.ToString() == AssemblyName.GetAssemblyName(path).Version.ToString())
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
            return existingAssemblies;
        }
    }
}
