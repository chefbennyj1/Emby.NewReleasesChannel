using System;
using System.Collections.Generic;
using System.IO;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using NewReleases.Configuration;

namespace NewReleases
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasThumbImage, IHasWebPages
    {
        public static Plugin Instance { get; private set; }
        public ImageFormat ThumbImageFormat => ImageFormat.Png;

        public override Guid Id => new Guid("33A1E636-0FDC-4EEA-8B3A-F2971274164B");
        public override string Name => "New Releases";

        public Stream GetThumbImage()
        {
            var type = GetType();
            return type.Assembly.GetManifestResourceStream(type.Namespace + ".thumb.png");
        }

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths,
            xmlSerializer)
        {
            Instance = this;
        }

        public IEnumerable<PluginPageInfo> GetPages() => new[]
        {
            new PluginPageInfo
            {
                Name                 = "NewReleasesPluginConfigurationPage",
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.NewReleasesPluginConfigurationPage.html"
            },
            new PluginPageInfo
            {
                Name                 = "NewReleasesPluginConfigurationPageJS",
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.NewReleasesPluginConfigurationPage.js"
            }
        };
    }
}