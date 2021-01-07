using Newtonsoft.Json;

namespace OpenBots.Agent.Core.Model
{
    public class NugetPackageSource
    {
        [JsonProperty("Enabled")]
        public bool? Enabled { get; set; }

        [JsonProperty("Package Name")]
        public string PackageName { get; set; }

        [JsonProperty("Package Source")]
        public string PackageSource { get; set; }
    }
}
