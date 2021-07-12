using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBots.Agent.Core.Model
{
    public class ServerConfiguration
    {
        public string OpenBotsOrchestrator { get; set; }
        public string OpenBotsOrganization { get; set; }
        public string OpenBotsUsername { get; set; }
        public string OpenBotsPassword { get; set; }
        public string OpenBotsProvisionKey { get; set; }
        public string OpenBotsHostname { get; set; }
        public string OpenBotsLogStorage { get; set; }
        public string OpenBotsLogLevel { get; set; }
        public string OpenBotsLogSink { get; set; }
        public string OpenBotsLogHttpURL { get; set; }
        public string OpenBotsLogFilePath { get; set; }
    }
}
