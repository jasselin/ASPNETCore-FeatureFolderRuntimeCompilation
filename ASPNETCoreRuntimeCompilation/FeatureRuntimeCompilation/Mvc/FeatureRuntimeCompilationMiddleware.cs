using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    public class FeatureRuntimeCompilationMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IRuntimeFeatureProvider _featureProvider;
        //private readonly IFeatureControllerInfoProvider _controllerInfoProvider;
        private readonly ApplicationPartManager _appPartManager;
        private readonly FeatureRuntimeCompilationActionDescriptorChangeProvider _actionDescriptorChangeProvider;
        private readonly ILogger<FeatureRuntimeCompilationMiddleware> _logger;

        public FeatureRuntimeCompilationMiddleware(RequestDelegate next, ILogger<FeatureRuntimeCompilationMiddleware> logger,
                                                   IRuntimeFeatureProvider featureProvider,
                                                   ApplicationPartManager appPartManager,
                                                   FeatureRuntimeCompilationActionDescriptorChangeProvider actionDescriptorChangeProvider)
        {
            _next = next;
            _logger = logger;
            _featureProvider = featureProvider;
            //_controllerInfoProvider = controllerInfoProvider;
            _appPartManager = appPartManager;
            _actionDescriptorChangeProvider = actionDescriptorChangeProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var requestPath = context.Request.Path;
            //var endpoint = context.GetEndpoint();
            //TODO: remove dependency on route data
            //var routeData = context.GetRouteData();
            //if (!routeData.Values.Any())
            //{
            //    _logger.LogWarning("No route values, skipping feature detection.");
            //    await _next(context);
            //    return;
            //}

            _logger.LogInformation($"Looking for feature for path '{context.Request.Path}'.");

            var feature = _featureProvider.GetFeature(context);
            if (feature == null)
            {
                _logger.LogWarning("No feature found.");
                await _next(context);
                return;
            }

            _logger.LogInformation($"Feature '{feature.Name}' was found.");

            if (!feature.HasChanged)
            {
                _logger.LogInformation("Feature has not changed, skipping compilation.");
                await _next(context);
                return;
            }

            _logger.LogInformation($"Feature has changed, compiling...");

            var parts = _appPartManager.ApplicationParts
                .OfType<AssemblyPart>()
                .Where(x => x.Assembly.ExportedTypes.Any(x => x.FullName == feature.ControllerType.FullName))
                .ToArray();

            foreach (var part in parts)
            {
                _logger.LogInformation($"Removing assembly '{part.Assembly.FullName}' from ApplicationPartManager.");
                _appPartManager.ApplicationParts.Remove(part);
            }

            _logger.LogInformation($"Adding assembly '{feature.Assembly.FullName} to ApplicationPartManager.'");
            _appPartManager.ApplicationParts.Add(new AssemblyPart(feature.Assembly));

            _logger.LogInformation("Triggering ActionDescriptonChangerProvider refresh.");
            _actionDescriptorChangeProvider.TokenSource.Cancel();

            //context.Items[FeatureRuntimeCompilation.HttpContextItemKey] = feature.ControllerType;

            await _next(context);
        }
    }
}
