using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASPNETCoreRuntimeCompilation.FeatureFolders
{
    public class FeatureMvcOptionsConfiguration : IConfigureOptions<MvcOptions>
    {
        private readonly ILogger<FeatureControllerModelConvention> _logger;

        public FeatureMvcOptionsConfiguration(ILogger<FeatureControllerModelConvention> logger)
        {
            _logger = logger;
        }

        public void Configure(MvcOptions options)
        {
            options.Conventions.Add(new FeatureControllerModelConvention(_logger));
        }
    }
}
