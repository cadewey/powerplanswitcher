namespace TheRefactory
{
    internal class PowerPlan
    {
        public string guid { get; private set; }
        public string name { get; private set; }

        public PowerPlan(string guid, string name)
        {
            this.guid = guid;
            this.name = name;
        }

    }
}