using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.Extensions.Logging;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class FeatureUpdater : IFeatureUpdater
    {
        private readonly ILogger<FeatureUpdater> _logger;

        private readonly List<FeatureUpdaterTask> _pendingUpdates = new List<FeatureUpdaterTask>();

        private readonly object _pendingUpdatesLock = new object();
        private readonly Timer _throttlingTimer;
        private bool _updateStarted;

        public FeatureUpdater(ILogger<FeatureUpdater> logger)
        {
            _logger = logger;

            _throttlingTimer = new Timer(2000)
            {
                AutoReset = false // fire once
            };
            _throttlingTimer.Elapsed += ProcessUpdates;
        }

        public void Update(FeatureMetadata metadata)
        {
            AddOrUpdateTask(metadata);

            if (!_updateStarted)
                StartThrottlingTimer();
            else
                _logger.LogInformation("Update in progress, skipping timer start.");
        }

        private void AddOrUpdateTask(FeatureMetadata metadata)
        {
            // Ensure thread safety while updating tasks to avoid duplication
            lock (_pendingUpdatesLock)
            {
                _logger.LogInformation($"Adding a task to update feature '{metadata.CacheKey}'.");

                // Pending task for the same feature is removed and added to the end of the list
                // Tasks that are not pending started compiling, so we add another task to the end of the list to update the feature again.
                var existingItem = _pendingUpdates.SingleOrDefault(x => x.Name.Equals(metadata.CacheKey) && x.Pending);
                if (existingItem != null)
                    _pendingUpdates.Remove(existingItem);

                var task = new FeatureUpdaterTask(metadata.CacheKey);
                _pendingUpdates.Add(task);
            }
        }

        private void StartThrottlingTimer()
        {
            _throttlingTimer.Stop();
            _throttlingTimer.Start();
        }

        private void ProcessUpdates(object sender, ElapsedEventArgs e)
        {
            _updateStarted = true;

            _logger.LogInformation("ProcessUpdates fired.");

            System.Threading.Thread.Sleep(2000);

            if (_pendingUpdates.Any())
                _pendingUpdates.First().Pending = true;

            while (_pendingUpdates.Any())
            {
                var task = _pendingUpdates.First();
                _logger.LogInformation($"Updating feature '{task.Name}'.");
                _pendingUpdates.Remove(task);
            }

            _logger.LogInformation("ProcessUpdates DONE!");

            _updateStarted = false;
        }
    }
}