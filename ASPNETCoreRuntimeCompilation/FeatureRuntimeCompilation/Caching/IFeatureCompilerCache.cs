using System.Reflection;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching
{
    public interface IFeatureCompilerCache
    {
        (FeatureCompilerCacheResult, bool NewAssembly) Get(string cacheKey); //TODO: change for metadata?
        (FeatureCompilerCacheResult, bool NewAssembly) GetOrAdd(string cacheKey, string featurePath); //TODO: change for metadata?
    }
}
