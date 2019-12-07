using System.Reflection;

namespace FeatureRuntimeCompilation.Mvc
{
    internal interface IFeatureApplicationPartManager
    {
        void Add(Assembly featureAssembly);
        void Remove(FeatureMetadata feature);
    }
}