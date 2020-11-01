using System;
using System.Collections.Generic;
using System.Management;

namespace OpenBots.Service.Client.Manager.Execution
{
    class WMIQueries
    {

        public static List<SessionView> GetInteractiveSessions()
        {
            List<SessionView> sessions = new List<SessionView>();
            string query = "Select * from Win32_LogonSession Where (LogonType = 2) OR (LogonType = 10)";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection sessionList = searcher.Get();

            foreach (ManagementObject obj in sessionList)
            {
                SessionView session = new SessionView();

                session.Name = obj.GetPropertyValue("Name")?.ToString();
                session.Caption = obj.GetPropertyValue("Caption")?.ToString();
                session.Description = obj.GetPropertyValue("Description")?.ToString();
                session.Status = obj.GetPropertyValue("Status")?.ToString();
                //session.StartTime = Convert.ToDateTime(obj.GetPropertyValue("StartTime"));
                session.AuthenticationPackage = obj.GetPropertyValue("AuthenticationPackage")?.ToString();
                session.LogonId = obj.GetPropertyValue("LogonId")?.ToString();
                session.LogonType = Convert.ToUInt32(obj.GetPropertyValue("LogonType"));

                sessions.Add(session);
            }

            return sessions;
        }

        public static List<ProcessView> GetProcessByName(string name)
        {
            List<ProcessView> processes = new List<ProcessView>();
            string query = "Select * from Win32_Process Where Name = \"" + name + "\"";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                ProcessView process = new ProcessView();

                process.ProcessId = Convert.ToUInt32(obj.GetPropertyValue("ProcessId"));
                process.Name = obj.GetPropertyValue("Name").ToString();
                process.Status = obj.GetPropertyValue("Status").ToString();
                process.SessionId = Convert.ToUInt32(obj.GetPropertyValue("SessionId"));

                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    process.OwnerDomain = argList[1];
                    process.OwnerUser = argList[0];
                    process.OwnerSid = argList[2];
                }
                processes.Add(process);
            }

            return processes;
        }


    }


    public class SessionView
    {
        public string Caption { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public string AuthenticationPackage { get; set; }
        public string LogonId { get; set; }
        public UInt32 LogonType { get; set; }
    }

    public class ProcessView
    {
        public UInt32 ProcessId { get; set; }
        public string Name { get; set; }

        public string Status { get; set; }

        public UInt32 SessionId { get; set; }

        public string OwnerUser { get; set; }

        public string OwnerDomain { get; set; }

        public string OwnerSid { get; set; }

    }
}
