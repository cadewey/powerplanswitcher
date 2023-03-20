using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlanSwitcher
{
    internal enum InitializationResult
    {
        Ok,
        Partial,
        Error
    }

    internal interface IGpuManager : IDisposable
    {
        string DeviceName { get; }

        (InitializationResult Result, string Error) Initialize();
        uint GetActivePowerLimit();
        int GetActivePowerLimitIndex();
        double[] GetAvailablePowerLimits();
        double SetPowerLimit(int powerLimitIndex);
        double CpuProfileChanged(int index);
        void ShowInfoForm();
    }
}
