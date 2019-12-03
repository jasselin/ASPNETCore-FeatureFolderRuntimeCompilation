using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Logging;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class RuntimeFeatureCompilationWatcher
    {
        private ConcurrentDictionary<string, List<Assembly>> _loadedAssemblies = new ConcurrentDictionary<string, List<Assembly>>();
        private readonly FeatureRuntimeCompilationActionDescriptorChangeProvider _onDemandActionDescriptorChangeProvider;
        private readonly ApplicationPartManager _applicationPartManager;
        private readonly ILogger<RuntimeFeatureCompilationWatcher> _logger;

        public RuntimeFeatureCompilationWatcher(FeatureRuntimeCompilationActionDescriptorChangeProvider onDemandActionDescriptorChangeProvider,
                ApplicationPartManager applicationPartManager, ILoggerFactory loggerFactory)
        {
            _onDemandActionDescriptorChangeProvider = onDemandActionDescriptorChangeProvider;
            _applicationPartManager = applicationPartManager;
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
            watcher.Filters.Add("*.cshtml");

            void changeEvent(object s, FileSystemEventArgs e)
            {
                _logger.LogInformation(e.ChangeType.ToString() + ": " + e.FullPath);
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