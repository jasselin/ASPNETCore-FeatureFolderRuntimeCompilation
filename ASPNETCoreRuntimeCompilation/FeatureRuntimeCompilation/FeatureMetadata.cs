namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class FeatureMetadata
    {
        public FeatureMetadata(string controllerTypeName, string featurePath)
        {
            ControllerTypeName = controllerTypeName;
            FeaturePath = featurePath;
        }

        public string ControllerTypeName { get; }
        public string FeaturePath { get; }
    }
}