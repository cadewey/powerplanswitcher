namespace TheRefactory
{
    internal class PowerPlan
    {
        public PowerPlan(string guid, string name)
        {
            Guid = guid;
            Name = name;
        }

        public string Guid { get; private set; }
        public string Name { get; private set; }
    }
}