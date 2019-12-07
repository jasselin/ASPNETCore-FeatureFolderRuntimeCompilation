namespace FeatureRuntimeCompilation
{
    internal interface IFeatureUpdater
    {
        void Update(FeatureMetadata metadata);
        bool UpdatePending();
    }
}