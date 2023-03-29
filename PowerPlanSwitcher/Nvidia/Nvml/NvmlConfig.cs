namespace PowerPlanSwitcher.Nvidia
{
    public class NvidiaAutoScaling
    {
        public int PlanIndex { get; set; }
        public double Scaling { get; set; }
    }

    public class NvidiaConfig
    {
        public string DriverSearchUrl { get; set; }
        public double? StartupPowerScaling { get; set; }
        public uint DeviceIndex { get; set; }
        public double[] AvailablePowerScaling { get; set; }
        public NvidiaAutoScaling[] PlanAutoScaling { get; set; }
    }
}
