using System.Reflection;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration
{
    public class FeatureRuntimeCompilationOptions
    {
        //TODO: Make this a list
        public Assembly Assembly { get; set; }

        public string ProjectPath { get; set; }
    }
}
