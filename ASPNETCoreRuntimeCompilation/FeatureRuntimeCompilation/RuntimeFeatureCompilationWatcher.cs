using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using System.Threading;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Logging;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class RuntimeFeatureCompilationWatcher
    {
        private ConcurrentDictionary<string, List<Assembly>> _loadedAssemblies = new ConcurrentDictionary<string, List<Assembly>>();
        private readonly FeatureRuntimeCompilationActionDescriptorChangeProvider _actionDescriptorChangeProvider;
        private readonly ApplicationPartManager _applicationPartManager;
        private readonly IRuntimeFeatureProvider _featureProvider;
        private readonly ILogger<RuntimeFeatureCompilationWatcher> _logger;

        public RuntimeFeatureCompilationWatcher(FeatureRuntimeCompilationActionDescriptorChangeProvider actionDescriptorChangeProvider,
                ApplicationPartManager applicationPartManager, ILoggerFactory loggerFactory, IRuntimeFeatureProvider compilerCache)
        {
            _actionDescriptorChangeProvider = actionDescriptorChangeProvider;
            _applicationPartManager = applicationPartManager;
            _featureProvider = compilerCache;
            _logger = loggerFactory.CreateLogger<RuntimeFeatureCompilationWatcher>();
        }

        public void Watch(FeatureRuntimeCompilationOptions options)
        {
            var configFolderPath = Path.Combine(Directory.GetCurrentDirectory(), options.FeaturesPath);

            if (!Directory.Exists(configFolderPath))
                Directory.CreateDirectory(configFolderPath);

            var watcher = new FileSystemWatcher()
            {
                Path = configFolderPath,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName,
                IncludeSubdirectories = true
            };

            watcher.Filters.Add("*.cs");
            //watcher.Filters.Add("*.cshtml");

            void changeEvent(object s, FileSystemEventArgs e)
            {
                _logger.LogInformation(e.ChangeType.ToString() + ": " + e.FullPath);

                var featureDirectory = Path.GetDirectoryName(e.FullPath);
                var feature = _featureProvider.GetFeature(featureDirectory);

                var featureAssemblyRegex = new Regex(@$"{feature.Name}.\w+-\w+\-\w+\-\w+\-\w+");

                var parts = _applicationPartManager.ApplicationParts
                    .OfType<AssemblyPart>()
                    .Where(x => featureAssemblyRegex.IsMatch(x.Name))
                    .ToArray();

                foreach (var part in parts)
                {
                    _logger.LogInformation($"Removing assembly '{part.Assembly.FullName}' from ApplicationPartManager.");
                    _applicationPartManager.ApplicationParts.Remove(part);
                }

                _logger.LogInformation($"Adding assembly '{feature.Assembly.FullName} to ApplicationPartManager.'");
                var assemblyPart = new AssemblyPart(feature.Assembly);
                _applicationPartManager.ApplicationParts.Add(assemblyPart);

                _logger.LogInformation("Triggering ActionDescriptonChangerProvider refresh.");
                _actionDescriptorChangeProvider.TokenSource.Cancel();
            }

            watcher.Created += changeEvent; // Create file
            watcher.Changed += changeEvent; // Rename file, Save file, Rename x.aaa -> x.cs
            watcher.Deleted += changeEvent; // Delete file
            watcher.Renamed += changeEvent; // Delete file

            // Not triggering: Delete or renaming directory containing cs, cshtml

            watcher.EnableRaisingEvents = true;
        }
    }
}