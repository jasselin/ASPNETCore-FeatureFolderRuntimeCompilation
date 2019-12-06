namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching
{
    public interface IFeatureChecksumGenerator
    {
        string GetChecksum(FeatureMetadata metadata);
    }
}
