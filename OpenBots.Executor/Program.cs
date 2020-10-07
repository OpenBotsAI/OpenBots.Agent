using System.Windows.Forms;

namespace OpenBots.Executor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                EngineHandler executor = new EngineHandler();
                executor.ExecuteScript(args[0].ToString());
            }
        }
    }
}
