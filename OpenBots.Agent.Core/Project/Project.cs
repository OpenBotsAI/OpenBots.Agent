using Newtonsoft.Json;
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
        public static List<string> DefaultCommands = new List<string>
        {
            "Data",
            "DataTable",
            "Dictionary",
            "Engine",
            "ErrorHandling",
            "Excel",
            "File",
            "Folder",
            "If",
            "Image",
            "Input",
            "List",
            "Loop",
            "Misc",
            "Outlook",
            "Process",
            "Switch",
            "Task",
            "TextFile",
            "Variable",
            "WebBrowser",
            "Window",
        };
    }
}
