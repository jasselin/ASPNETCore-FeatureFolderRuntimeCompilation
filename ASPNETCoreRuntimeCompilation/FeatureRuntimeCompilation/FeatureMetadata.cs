namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class FeatureMetadata
    {
        public FeatureMetadata(string featureName, string controllerTypeName, string featurePath)
        {
            FeatureName = featureName.ToLower(); // Case-sensitive cache key
            ControllerTypeName = controllerTypeName;
            FeaturePath = featurePath;
        }

        public string FeatureName { get; }
        public string ControllerTypeName { get; }
        public string FeaturePath { get; }
    }
}