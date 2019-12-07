using System.Reflection;

namespace FeatureRuntimeCompilation.Mvc
{
    public interface IFeatureApplicationPartManager
    {
        void Add(Assembly featureAssembly);
        void Remove(FeatureMetadata feature);
    }
}