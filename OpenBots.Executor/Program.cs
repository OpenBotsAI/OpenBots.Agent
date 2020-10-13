
using Newtonsoft.Json;
using OpenBots.Agent.Core.Model;
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
                JobExecutionParams executionParams = JsonConvert.DeserializeObject<JobExecutionParams>(args[0].ToString());

                EngineHandler executor = new EngineHandler();
                executor.ExecuteScript(executionParams);
            }
        }
    }
}
