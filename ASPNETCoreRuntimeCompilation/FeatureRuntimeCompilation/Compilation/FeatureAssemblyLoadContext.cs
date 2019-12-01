using System;
using System.Reflection;
using System.Runtime.Loader;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation
{
    public class FeatureAssemblyLoadContext : AssemblyLoadContext
    {
        public FeatureAssemblyLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return base.Load(assemblyName);
        }
    }
}
