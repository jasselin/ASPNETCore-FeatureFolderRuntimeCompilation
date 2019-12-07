namespace FeatureRuntimeCompilation.Caching
{
    internal interface IFeatureChecksumGenerator
    {
        string GetChecksum(FeatureMetadata metadata);
    }
}
