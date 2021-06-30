using System.ComponentModel.DataAnnotations;

namespace OpenBots.Server.SDK.Api
{
    public class ConnectAgentViewModel
    {
        [Required]
        public string MachineName { get; set; }
        public string MacAddresses { get; set; }
    }
}
