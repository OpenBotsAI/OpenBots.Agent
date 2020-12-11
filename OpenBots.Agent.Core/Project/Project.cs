using System;
using System.Collections.Generic;

namespace OpenBots.Agent.Core.Project
{
    public class Project
    {
        public Guid ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Main { get; set; }
        public string Version { get; set; }
        public Dictionary<string, string> Dependencies { get; set; }
    }
}
