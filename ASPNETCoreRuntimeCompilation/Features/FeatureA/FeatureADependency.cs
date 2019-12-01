namespace ASPNETCoreRuntimeCompilation.Features.FeatureA
{
    public class FeatureADependency : IFeatureADependency
    {
        public string GetMessage() => "Message A dependency message!";
    }
}