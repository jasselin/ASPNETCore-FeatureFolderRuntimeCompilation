namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class FeatureMetadata
    {
        public FeatureMetadata(string featureName, string controllerTypeName, string featurePath)
        {
            Name = featureName.ToLower(); // Case-sensitive cache key
            ControllerTypeName = controllerTypeName;
            FeaturePath = featurePath;
        }

        public string Name { get; }
        public string ControllerTypeName { get; }
        public string FeaturePath { get; }
    }
}