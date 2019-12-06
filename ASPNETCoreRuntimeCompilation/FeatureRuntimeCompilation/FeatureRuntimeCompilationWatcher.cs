using System.IO;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using Microsoft.Extensions.Logging;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class FeatureRuntimeCompilationWatcher
    {
        private readonly IFeatureMetadataProvider _metadataProvider;
        private readonly IFeatureUpdater _featureUpdater;
        private readonly ILogger<FeatureRuntimeCompilationWatcher> _logger;

        public FeatureRuntimeCompilationWatcher(IFeatureMetadataProvider metadataProvider,
                IFeatureUpdater featureUpdater, ILogger<FeatureRuntimeCompilationWatcher> logger)
        {
            _metadataProvider = metadataProvider;
            _featureUpdater = featureUpdater;
            _logger = logger;
        }

        public void Watch(FeatureRuntimeCompilationOptions options)
        {
            var configFolderPath = Path.Combine(Directory.GetCurrentDirectory(), options.FeaturesPath);

            var watcher = new FileSystemWatcher()
            {
                Path = configFolderPath,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName,
                IncludeSubdirectories = true,
                Filter = "*.cs"
            };

            void changeEvent(object s, FileSystemEventArgs e)
            {
                _logger.LogInformation(e.ChangeType.ToString() + ": " + e.FullPath);

                var featurePath = Path.GetDirectoryName(e.FullPath);
                var metadata = _metadataProvider.GetMetadataFor(featurePath);

                _featureUpdater.Update(metadata);
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