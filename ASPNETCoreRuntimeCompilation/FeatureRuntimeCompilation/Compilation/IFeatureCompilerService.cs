namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation
{
    public interface IFeatureCompilerService
    {
        FeatureCompilerResult Compile(string cacheKey, string featurePath);
    }
}
