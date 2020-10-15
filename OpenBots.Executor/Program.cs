
using Newtonsoft.Json;
using OpenBots.Agent.Core.Model;
using System;
using System.Text;
using System.Windows.Forms;


namespace OpenBots.Executor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                // Get Execution Parameters
                var paramsJsonString = Encoding.UTF8.GetString(Convert.FromBase64String(args[0].ToString()));
                JobExecutionParams executionParams = JsonConvert.DeserializeObject<JobExecutionParams>(paramsJsonString);

                EngineHandler executor = new EngineHandler();
                executor.ExecuteScript(executionParams);
            }
        }
    }
}
