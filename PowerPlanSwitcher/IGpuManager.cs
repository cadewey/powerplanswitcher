using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlanSwitcher
{
    internal interface IGpuManager : IDisposable
    {
        bool Initialize(out string errorMessage);
        string GetDeviceName();
        double GetPowerLimit();
        int GetActivePowerLimitIndex();
        double[] GetAvailablePowerLimits();
        double SetPowerLimit(int powerLimitIndex);
        double CpuProfileChanged(int index);
    }
}
