using System.Collections.Generic;

namespace OpenBots.Server.SDK.Model
{
    public class NextJobViewModel
    {
        public bool IsJobAvailable { get; set; }
        public Job AssignedJob { get; set; }
        public IEnumerable<JobParameter> JobParameters { get; set; }
    }
}
