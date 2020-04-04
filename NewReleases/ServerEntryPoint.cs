using System;
using MediaBrowser.Controller.Plugins;

namespace NewReleases
{
    public class ServerEntryPoint : IServerEntryPoint
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            Plugin.Instance.UpdateConfiguration(Plugin.Instance.Configuration);
        }
    }
}
