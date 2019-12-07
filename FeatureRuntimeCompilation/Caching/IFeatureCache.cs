using FeatureRuntimeCompilation.Compilation;

namespace FeatureRuntimeCompilation.Caching
{
    public interface IFeatureCache
    {
        FeatureCompilerResult Get(FeatureMetadata feature);
        (FeatureCompilerResult, bool hasUpdated) GetOrUpdate(FeatureMetadata feature);
    }
}
