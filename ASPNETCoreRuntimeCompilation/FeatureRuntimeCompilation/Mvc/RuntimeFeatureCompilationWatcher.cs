using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    public class RuntimeFeatureCompilationWatcher
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<RuntimeFeatureCompilationWatcher> _logger;
        private readonly IFeatureCompilerService _compilerService;

        public RuntimeFeatureCompilationWatcher(RequestDelegate next, ILogger<RuntimeFeatureCompilationWatcher> logger,
            IFeatureCompilerService compilerService)
        {
            _next = next;
            _logger = logger;
            _compilerService = compilerService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //_logger.LogInformation("Waiting for pending compilation...");

            //var timeout = 10000;
            //var retryEvery = timeout / 20;
            //var i = 0;
            //while (_compilerService.CompilationPending() && i < timeout / 20)
            //{
            //    System.Threading.Thread.Sleep(retryEvery);
            //    i++;
            //}

            //_logger.LogInformation("Compilation complete.");

            await _next(context);
        }
    }
}
