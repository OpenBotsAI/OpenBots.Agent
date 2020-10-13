namespace OpenBots.Agent.Core.Model
{
    public class JobExecutionParams
    {
        public string ProcessName { get; set; }
        public string MainFilePath { get; set; }
        public string ProjectDirectoryPath { get; set; }
        public ServerConnectionSettings ServerConnectionSettings { get; set; }
    }
}
