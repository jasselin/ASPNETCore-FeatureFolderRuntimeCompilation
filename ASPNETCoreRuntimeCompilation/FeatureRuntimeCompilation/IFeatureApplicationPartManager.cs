using System.Reflection;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public interface IFeatureApplicationPartManager
    {
        void Add(Assembly featureAssembly);
        void Remove(FeatureMetadata feature);
    }
}