using System;

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
