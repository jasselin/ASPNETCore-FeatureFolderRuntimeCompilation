using FeatureRuntimeCompilation.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FeatureRuntimeCompilation.FeatureFolders
{
    internal class FeatureMvcOptionsConfiguration : IConfigureOptions<MvcOptions>
    {
        private readonly FeatureFolderOptions _options;
        private readonly ILogger<FeatureControllerModelConvention> _logger;

        public FeatureMvcOptionsConfiguration(FeatureFolderOptions options, ILogger<FeatureControllerModelConvention> logger)
        {
            _options = options;
            _logger = logger;
        }

        public void Configure(MvcOptions options) => options.Conventions.Add(new FeatureControllerModelConvention(_options, _logger));
    }
}
