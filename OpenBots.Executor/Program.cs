
using Newtonsoft.Json;
using OpenBots.Agent.Core.Model;
using OpenBots.Agent.Core.Utilities;
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
                var paramsJsonString = DataFormatter.DecompressString(args[0].ToString());
                JobExecutionParams executionParams = JsonConvert.DeserializeObject<JobExecutionParams>(paramsJsonString);

                EngineHandler executor = new EngineHandler();
                executor.LoadProjectAssemblies(executionParams.ProjectDependencies);
                executor.ExecuteScript(executionParams);
            }
        }
    }
}
