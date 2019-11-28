using System.Reflection;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration
{
    public class FeatureRuntimeCompilationOptions
    {
        //TODO: Implement ProjectPath for assembly, replace IWebHostEnvironment references
        //TODO: Make this a list
        public Assembly Assembly { get; set; }

        public string ProjectPath { get; set; }
    }
}
