namespace ASPNETCoreRuntimeCompilation.Features.FeatureA
{
    public class FeatureAViewModel
    {
        public FeatureAViewModel()
        {
            SubViewModel = new FeatureASubViewModel();
        }

        public string Message { get; set; }
        public int InputText { get; set; }
        //public string InputText { get; set; }

        public FeatureASubViewModel SubViewModel { get; set; }
    }
}
