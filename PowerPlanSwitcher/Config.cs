using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlanSwitcher
{
    public class Config
    {
        public bool EnableRPCServer { get; set; }
        public int StartupPlanIndex { get; set; }
        public Dictionary<string, string> PlanIconColors { get; set; }
        public List<GpuConfig> Gpus { get; set; }
        public List<ProcessAutoSwitchingConfig> ProcessAutoSwitching { get; set; }
    }
}
