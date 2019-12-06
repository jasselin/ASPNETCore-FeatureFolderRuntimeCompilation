namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class FeatureUpdaterTask
    {
        public FeatureUpdaterTask(FeatureMetadata metadata)
        {
            Feature = metadata;
            Pending = true;
        }
        public string Name => Feature.Name;
        public FeatureMetadata Feature { get; }
        public bool Pending { get; set; }
    }
}