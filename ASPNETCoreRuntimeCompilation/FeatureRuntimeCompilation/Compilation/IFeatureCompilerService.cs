namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation
{
    public interface IFeatureCompilerService
    {
        FeatureCompilerResult Compile(string featurePath);
    }
}
