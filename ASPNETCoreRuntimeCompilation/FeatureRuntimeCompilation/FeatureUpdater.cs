using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class FeatureUpdater : IFeatureUpdater
    {
        private readonly IFeatureCache _featureCache;
        private readonly IFeatureApplicationPartManager _featureAppPartManager;
        private readonly FeatureRuntimeCompilationActionDescriptorChangeProvider _actionDescriptorChangeProvider;
        private readonly ILogger<FeatureUpdater> _logger;

        private readonly List<FeatureMetadata> _pendingUpdates = new List<FeatureMetadata>();

        private readonly object _pendingUpdatesLock = new object();
        private readonly Timer _throttlingTimer;
        private bool _updatePending;

        public FeatureUpdater(IFeatureCache featureCache,
            IFeatureApplicationPartManager featureAppPartManager,
            FeatureRuntimeCompilationActionDescriptorChangeProvider actionDescriptorChangeProvider,
            ILogger<FeatureUpdater> logger)
        {
            _featureCache = featureCache;
            _featureAppPartManager = featureAppPartManager;
            _actionDescriptorChangeProvider = actionDescriptorChangeProvider;
            _logger = logger;

            _throttlingTimer = new Timer(200) //TODO: lower
            {
                AutoReset = false // fire once
            };
            _throttlingTimer.Elapsed += StartProcessingUpdates;
        }

        public void Update(FeatureMetadata feature)
        {
            AddOrUpdateTask(feature);

            if (!_updatePending)
                StartThrottlingTimer();
            else
                _logger.LogInformation("Update in progress, skipping timer start.");
        }

        private void AddOrUpdateTask(FeatureMetadata feature)
        {
            // Ensure thread safety while updating tasks to avoid duplication
            lock (_pendingUpdatesLock)
            {
                _logger.LogInformation($"Adding a task to update feature '{feature.Name}'.");

                // Pending task for the same feature is removed and added to the end of the list
                // Tasks that are not pending started compiling, so we add another task to the end of the list to update the feature again.
                var existingItem = _pendingUpdates.SingleOrDefault(x => x.Name.Equals(feature.Name));
                if (existingItem != null)
                    _pendingUpdates.Remove(existingItem);

                _pendingUpdates.Add(feature);
            }
        }

        private void StartThrottlingTimer()
        {
            _throttlingTimer.Stop();
            _throttlingTimer.Start();
        }

        private void StartProcessingUpdates(object sender, ElapsedEventArgs e)
        {
            _updatePending = true;

            _logger.LogDebug("ProcessUpdates fired.");

            // Not optimal, but at least tasks are batched. Might optimize with continuous updates later.
            while (_pendingUpdates.Any())
                ProcessUpdates();

            _logger.LogDebug("ProcessUpdates done.");

            _updatePending = false;
        }

        private void ProcessUpdates()
        {
            Task[] tasks = null;
            lock (_pendingUpdatesLock)
            {
                tasks = _pendingUpdates
                    .Select(x => Task.Run(() => UpdateFeature(x)))
                    .ToArray();
            }

            Task.WaitAll(tasks);
        }

        private void UpdateFeature(FeatureMetadata feature)
        {
            lock (_pendingUpdatesLock)
            {
                _pendingUpdates.Remove(feature);
            }

            _logger.LogInformation($"Updating feature '{feature.Name}'.");

            var sw = Stopwatch.StartNew();
            var result = _featureCache.GetOrUpdate(feature);
            if (!result.Success)
            {
                _logger.LogInformation("Compilation failed, skipping feature update.");
                return;
            }
            sw.Stop();
            _logger.LogInformation($"Feature '{feature.Name}' compiled in {sw.ElapsedMilliseconds}ms.");

            _featureAppPartManager.Remove(feature);
            _featureAppPartManager.Add(result.Assembly);

            _actionDescriptorChangeProvider.TokenSource.Cancel();

            _logger.LogInformation($"Feature '{feature.Name}' updated.");
        }

        public bool UpdatePending()
        {
            return _pendingUpdates.Any();
        }
    }
}