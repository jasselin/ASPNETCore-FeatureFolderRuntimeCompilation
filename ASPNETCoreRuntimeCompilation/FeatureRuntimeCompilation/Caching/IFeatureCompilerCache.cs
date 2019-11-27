namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching
{
    public interface IFeatureCompilerCache
    {
        (FeatureCompilerCacheResult, bool NewAssembly) GetOrAdd(string cacheKey, string featurePath);
    }
}
