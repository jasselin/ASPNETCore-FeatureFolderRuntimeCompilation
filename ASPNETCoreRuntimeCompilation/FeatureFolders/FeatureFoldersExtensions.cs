using Microsoft.Extensions.DependencyInjection;

namespace ASPNETCoreRuntimeCompilation.FeatureFolders
{
    public static class FeatureFoldersExtensions
    {
        public static IMvcBuilder AddFeatureFolders(this IMvcBuilder mvcBuilder)
        {
            return mvcBuilder
                .AddMvcOptions(options =>
                {
                    options.Conventions.Add(new FeatureControllerModelConvention());
                })
                .AddRazorOptions(options =>
                {
                    // Relocate the default view locations to the 'Features' folder.
                    options.ViewLocationFormats.Add("/Features/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Features/Shared/{0}.cshtml");

                    options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
                });
        }
    }
}
