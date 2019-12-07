namespace FeatureRuntimeCompilation
{
    public interface IFeatureUpdater
    {
        void Update(FeatureMetadata metadata);
        bool UpdatePending();
    }
}