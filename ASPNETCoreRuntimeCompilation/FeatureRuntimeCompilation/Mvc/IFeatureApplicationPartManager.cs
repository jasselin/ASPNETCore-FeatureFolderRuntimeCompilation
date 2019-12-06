using System.Reflection;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    public interface IFeatureApplicationPartManager
    {
        void Add(Assembly featureAssembly);
        void Remove(FeatureMetadata feature);
    }
}