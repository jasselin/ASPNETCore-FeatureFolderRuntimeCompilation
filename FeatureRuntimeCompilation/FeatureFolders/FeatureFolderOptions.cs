namespace FeatureRuntimeCompilation.FeatureFolders
{
    public class FeatureFolderOptions
    {
        public FeatureFolderOptions(string featureNamespace)
        {
            FeatureNamespace = featureNamespace;
        }

        public string FeatureNamespace { get; }
    }
}
