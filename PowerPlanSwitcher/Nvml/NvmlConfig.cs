namespace PowerPlanSwitcher.Nvml
{
    public class NvmlAutoScaling
    {
        public int PlanIndex { get; set; }
        public double Scaling { get; set; }
    }

    public class NvmlConfig
    {
        public uint DeviceIndex { get; set; }
        public double[] AvailablePowerScaling { get; set; }
        public NvmlAutoScaling[] PlanAutoScaling { get; set; }
    }
}
