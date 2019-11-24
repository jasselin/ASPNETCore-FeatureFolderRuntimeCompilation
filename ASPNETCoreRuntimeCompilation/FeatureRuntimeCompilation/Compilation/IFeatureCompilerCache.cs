namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation
{
    public interface IFeatureCompilerCache
    {
        FeatureCompilerCacheResult GetOrAdd(string controllerTypeName, string featurePath);
    }
}
