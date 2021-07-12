using Newtonsoft.Json;

namespace OpenBots.Agent.Core.Utilities
{
    public static class HelperMethods
    {
        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
