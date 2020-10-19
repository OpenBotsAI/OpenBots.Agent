using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenBots.Agent.Core.Utilities
{
    public static class ExceptionSerializer
    {
        public static string Serialize(Exception ex)
        {
            var dict = new Dictionary<string, object>();

            var type = ex.GetType();
            dict["ClassName"] = type.FullName;
            dict["Message"] = ex.Message;
            dict["Data"] = ex.Data;
            dict["InnerException"] = ex.InnerException;
            dict["HResult"] = ex.HResult;
            dict["Source"] = ex.Source;

            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!dict.ContainsKey(p.Name))
                    dict[p.Name] = p.GetValue(ex);
            }

            return JsonConvert.SerializeObject(dict);
        }
    }
}
