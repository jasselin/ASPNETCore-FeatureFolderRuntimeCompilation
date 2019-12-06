using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching
{
    public interface IFeatureCache
    {
        FeatureCompilerResult Get(FeatureMetadata feature);
        FeatureCompilerResult GetOrUpdate(FeatureMetadata feature);
    }
}
