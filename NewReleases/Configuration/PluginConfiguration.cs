using System;
using MediaBrowser.Model.Plugins;

namespace NewReleases.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string MinPremiereDate { get; set; }
    }
}
