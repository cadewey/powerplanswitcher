namespace PowerPlanSwitcher.Nvidia
{
    public class NvmlAutoScaling
    {
        public int PlanIndex { get; set; }
        public double Scaling { get; set; }
    }

    public class NvmlConfig
    {
        public double? StartupPowerScaling { get; set; }
        public uint DeviceIndex { get; set; }
        public double[] AvailablePowerScaling { get; set; }
        public NvmlAutoScaling[] PlanAutoScaling { get; set; }
    }
}
