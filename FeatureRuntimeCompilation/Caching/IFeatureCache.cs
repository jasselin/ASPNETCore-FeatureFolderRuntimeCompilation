using FeatureRuntimeCompilation.Compilation;

namespace FeatureRuntimeCompilation.Caching
{
    internal interface IFeatureCache
    {
        FeatureCompilerResult Get(FeatureMetadata feature);
        (FeatureCompilerResult, bool hasUpdated) GetOrUpdate(FeatureMetadata feature);
    }
}
