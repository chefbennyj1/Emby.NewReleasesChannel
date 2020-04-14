using System;
using MediaBrowser.Model.Plugins;

namespace NewReleases.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string MinPremiereDate { get; set; }
        public bool IncrementMonths { get; set; }
        public bool IncrementDays { get; set; }
    }
}
