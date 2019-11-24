using ASPNETCoreRuntimeCompilation.FeatureFolders;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            //TODO: Handle putting components in another assembly

            //TODO: No longer a middleware, keep there?
            // Feature runtime compilation middleware, before hitting the endpoints
            app.UseFeatureRuntimeCompilation();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute(); // Attribute routing
            });
        }
    }
}