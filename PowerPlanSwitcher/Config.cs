using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlanSwitcher
{
    public class Config
    {
        public int StartupPlanIndex { get; set; }
        public List<GpuConfig> Gpus { get; set; }
        public List<ProcessAutoSwitchingConfig> ProcessAutoSwitching { get; set; }
    }
}
