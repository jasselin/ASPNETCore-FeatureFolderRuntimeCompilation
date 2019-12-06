namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class FeatureUpdaterTask
    {
        public FeatureUpdaterTask(string name)
        {
            Name = name;
            Pending = true;
        }
        public string Name { get; }
        public bool Pending { get; set; }
    }
}