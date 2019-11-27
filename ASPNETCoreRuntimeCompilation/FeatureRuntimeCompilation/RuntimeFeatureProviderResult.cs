using System;
using System.Reflection;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class RuntimeFeatureProviderResult
    {
        public RuntimeFeatureProviderResult(Assembly assembly, bool isUpdated, Type controllerType)
        {
            Assembly = assembly;
            HasChanged = isUpdated;
            ControllerType = controllerType;
        }

        public Assembly Assembly { get; }
        public bool HasChanged { get; }
        public Type ControllerType { get; }
    }
}