using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    public class FeatureRuntimeCompilationMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<FeatureRuntimeCompilationMiddleware> _logger;
        private readonly IFeatureUpdater _featureUpdater;

        public FeatureRuntimeCompilationMiddleware(RequestDelegate next, IFeatureUpdater featureUpdater,
            ILogger<FeatureRuntimeCompilationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _featureUpdater = featureUpdater;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Nothing to wait for, moving on.
            if (!_featureUpdater.UpdatePending())
            {
                await _next(context);
                return;
            }

            _logger.LogInformation("Waiting for pending updates...");

            var timeout = 10000;
            var retryEvery = timeout / 20;
            var i = 0;
            while (_featureUpdater.UpdatePending() && i < timeout / 20)
            {
                System.Threading.Thread.Sleep(retryEvery);
                i++;
            }

            if (_featureUpdater.UpdatePending())
                _logger.LogWarning("Timeout limit exceeded.");
            else
                _logger.LogInformation("All updates completed.");


            //context.RequestServices = // replace default container with feature container

            await _next(context);
        }
    }
}
