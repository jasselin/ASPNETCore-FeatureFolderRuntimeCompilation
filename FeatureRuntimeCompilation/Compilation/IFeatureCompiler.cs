namespace FeatureRuntimeCompilation.Compilation
{
    public interface IFeatureCompiler
    {
        FeatureCompilerResult Compile(string assemblyName, string featurePath, string checksum);
    }
}
