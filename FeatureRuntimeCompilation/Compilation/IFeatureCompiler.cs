namespace FeatureRuntimeCompilation.Compilation
{
    internal interface IFeatureCompiler
    {
        FeatureCompilerResult Compile(string assemblyName, string featurePath, string checksum);
    }
}
