using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration
{
    public static class FeatureRuntimeCompilationExtensions
    {
        public static IMvcBuilder AddFeatureRuntimeCompilation(this IMvcBuilder mvcBuilder, FeatureRuntimeCompilationOptions options)
        {
            var services = mvcBuilder.Services;

            // ASP.NET (Http, Routing, Razor)
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<EndpointSelector, FeatureEndpointSelector>();
            services.AddSingleton<RazorReferenceManager, FeatureRazorReferenceManager>();
            services.AddSingleton<FeatureActionDescriptorChangeProvider>();
            services.AddSingleton<IActionDescriptorChangeProvider>(sp => sp.GetService<FeatureActionDescriptorChangeProvider>());

            mvcBuilder.AddRazorRuntimeCompilation(options);

            // Compilation
            services.AddSingleton(options);
            services.AddSingleton<FeatureRuntimeCompilationWatcher>();
            services.AddTransient<IFeatureMetadataProvider, FeatureMetadataProvider>();
            services.AddSingleton<IFeatureUpdater, FeatureUpdater>();
            services.AddSingleton<IFeatureApplicationPartManager, FeatureApplicationPartManager>();
            services.AddSingleton<IFeatureCache, FeatureCache>();
            services.AddSingleton<IFeatureCompiler, FeatureCompiler>();
            services.AddTransient<IFeatureChecksumGenerator, FeatureChecksumGenerator>();
            services.AddSingleton<IFeatureChangeTokenProvider, FeatureChangeTokenProvider>();

            // Setup
            if (Directory.Exists(options.AssembliesOutputPath))
                Directory.Delete(options.AssembliesOutputPath, true);

            Directory.CreateDirectory(options.AssembliesOutputPath);

            FeatureAssemblyLocator.Init();

            return mvcBuilder;
        }

        private static void AddRazorRuntimeCompilation(this IMvcBuilder mvcBuilder, FeatureRuntimeCompilationOptions options)
        {
            RazorRuntimeCompilationMvcCoreBuilderExtensions.AddServices(mvcBuilder.Services);
            mvcBuilder.Services.ConfigureOptions<MvcRazorRuntimeCompilationOptionsConfiguration>();
        }

        public static IApplicationBuilder UseFeatureRuntimeCompilation(this IApplicationBuilder app)
        {
            var watcher = app.ApplicationServices.GetRequiredService<FeatureRuntimeCompilationWatcher>();
            var options = app.ApplicationServices.GetRequiredService<FeatureRuntimeCompilationOptions>();
            Task.Run(() => watcher.Watch(options));

            app.UseMiddleware<FeatureRuntimeCompilationMiddleware>();

            return app;
        }
    }
}
