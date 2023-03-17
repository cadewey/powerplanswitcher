using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace PowerPlanSwitcher
{
    public enum GpuType
    {
        Nvml
    }

    public class GpuConfig
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public GpuType Type { get; set; }
        public JObject Config { get; set; }
    }

    public class GpuConfigList
    {
        public List<GpuConfig> Gpus { get; set; }
    }
}
