using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching
{
    public interface IFeatureCache
    {
        FeatureCompilerResult Get(FeatureMetadata feature);
        (FeatureCompilerResult, bool hasUpdated) GetOrUpdate(FeatureMetadata feature);
    }
}
