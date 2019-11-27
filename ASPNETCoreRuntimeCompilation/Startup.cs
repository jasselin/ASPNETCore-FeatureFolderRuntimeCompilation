using ASPNETCoreRuntimeCompilation.FeatureFolders;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASPNETCoreRuntimeCompilation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddFeatureFolders()
                .AddFeatureRuntimeCompilation(new FeatureRuntimeCompilationOptions
                {
                    Assembly = typeof(Startup).Assembly // Assembly to be dynamically compiled at runtime
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            var featureProvider = app.ApplicationServices.GetRequiredService<IRuntimeFeatureProvider>();
            var appPartManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();
            var actionDescriptorChangeProvider = app.ApplicationServices.GetRequiredService<FeatureRuntimeCompilationActionDescriptorChangeProvider>();
            app.UseMiddleware<FeatureRuntimeCompilationMiddleware>(featureProvider, appPartManager, actionDescriptorChangeProvider); //TODO: Extension method

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute(); // Attribute routing
            });

            //TODO: Handle putting components in another assembly

            // Removes dynamically compiled assemblies from ApplicationPartManager after ControllerModelConvention are applied.
            app.UseFeatureRuntimeCompilation(); // TODO: refactor, merge with previous extension method
        }
    }
}