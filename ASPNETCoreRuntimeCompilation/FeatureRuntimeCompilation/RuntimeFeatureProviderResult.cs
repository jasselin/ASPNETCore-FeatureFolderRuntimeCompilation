using System;
using System.Reflection;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class RuntimeFeatureProviderResult
    {
        public RuntimeFeatureProviderResult(Assembly assembly, bool isUpdated, Type controllerType)
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            HasChanged = isUpdated;
            ControllerType = controllerType ?? throw new ArgumentNullException(nameof(assembly));
            Name = assembly.GetName().Name.Substring(0, assembly.GetName().Name.LastIndexOf("."));
        }

        public string Name { get; }
        public Assembly Assembly { get; }
        public bool HasChanged { get; }
        public Type ControllerType { get; }
    }
}