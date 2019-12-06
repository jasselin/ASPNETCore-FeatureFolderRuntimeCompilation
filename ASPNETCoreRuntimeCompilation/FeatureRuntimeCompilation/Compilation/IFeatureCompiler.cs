namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation
{
    public interface IFeatureCompiler
    {
        FeatureCompilerResult Compile(string cacheKey, string featurePath);
    }
}
