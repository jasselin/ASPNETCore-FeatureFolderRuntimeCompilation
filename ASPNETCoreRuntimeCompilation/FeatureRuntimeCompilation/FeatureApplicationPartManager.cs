using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Logging;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class FeatureApplicationPartManager : IFeatureApplicationPartManager
    {
        private readonly ApplicationPartManager _appPartManager;
        private readonly ILogger<FeatureApplicationPartManager> _logger;

        public FeatureApplicationPartManager(ApplicationPartManager appPartManager, 
            ILogger<FeatureApplicationPartManager> logger)
        {
            _appPartManager = appPartManager;
            _logger = logger;
        }
        
        public void Add(Assembly featureAssembly)
        {
            _logger.LogInformation($"Adding assembly '{featureAssembly.FullName} to ApplicationPartManager.'");
            var assemblyPart = new AssemblyPart(featureAssembly);
            _appPartManager.ApplicationParts.Add(assemblyPart);
        }

        public void Remove(FeatureMetadata feature)
        {
            var featureAssemblyRegex = new Regex(@$"{feature.Name}.\w+-\w+\-\w+\-\w+\-\w+");

            var parts = _appPartManager.ApplicationParts
                .OfType<AssemblyPart>()
                .Where(x => featureAssemblyRegex.IsMatch(x.Name))
                .ToArray();

            foreach (var part in parts)
            {
                _logger.LogInformation($"Removing assembly '{part.Assembly.FullName}' from ApplicationPartManager.");
                _appPartManager.ApplicationParts.Remove(part);
            }
        }
    }
}