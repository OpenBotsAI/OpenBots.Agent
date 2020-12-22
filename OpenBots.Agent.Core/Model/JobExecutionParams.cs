using System.Collections.Generic;

namespace OpenBots.Agent.Core.Model
{
    public class JobExecutionParams
    {
        public string JobId { get; set; }
        public string AutomationId { get; set; }
        public string AutomationName { get; set; }
        public string MainFilePath { get; set; }
        public string ProjectDirectoryPath { get; set; }
        public List<string> ProjectDependencies { get; set; }
        public ServerConnectionSettings ServerConnectionSettings { get; set; }
    }
}
