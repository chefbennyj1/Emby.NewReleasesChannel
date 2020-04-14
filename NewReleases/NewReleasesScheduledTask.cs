using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Tasks;
using NewReleases.Configuration;

namespace NewReleases
{
    public class NewReleasesScheduledTask : IScheduledTask, IConfigurableScheduledTask
    {
        private ITaskManager TaskManager { get; set; }

        public NewReleasesScheduledTask(ITaskManager taskMan)
        {
            TaskManager = taskMan;
        }
        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            var config = Plugin.Instance.Configuration;
            if (config.MinPremiereDate != null)
            {
                if (config.IncrementMonths == true)
                {
                    var currentDate = DateTimeOffset.Parse(config.MinPremiereDate);
                    config.MinPremiereDate = currentDate.AddMonths(1).ToString();

                    Plugin.Instance.UpdateConfiguration(new PluginConfiguration()
                    {
                        MinPremiereDate = config.MinPremiereDate,
                    });

                    foreach (var t in TaskManager.ScheduledTasks)
                    {
                        if (t.Name == "Refresh Internet Channels")
                        {
                            await TaskManager.Execute(t, new TaskOptions());
                        }
                    }

                }

                if (config.IncrementDays == true)
                {
                    var currentDate = DateTimeOffset.Parse(config.MinPremiereDate);
                    config.MinPremiereDate = currentDate.AddDays(1).ToString();

                    Plugin.Instance.UpdateConfiguration(new PluginConfiguration()
                    {
                        MinPremiereDate = config.MinPremiereDate,
                    });

                    foreach (var t in TaskManager.ScheduledTasks)
                    {
                        if (t.Name == "Refresh Internet Channels")
                        {
                            await TaskManager.Execute(t, new TaskOptions());
                        }
                    }
                }
            }
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[]
            {
                new TaskTriggerInfo
                {
                    Type          = TaskTriggerInfo.TriggerInterval,
                    IntervalTicks = TimeSpan.FromDays(1).Ticks
                }
            };
        }

        public string Name        => "New Releases Monthly Premiere Update";
        public string Key         => "New Releases";
        public string Description => "Increment the New Releases Minimum Premiere Date Monthly.";
        public string Category    => "Library";
        public bool IsHidden      => true;
        public bool IsEnabled     => true;
        public bool IsLogged      => true;
    }
}
