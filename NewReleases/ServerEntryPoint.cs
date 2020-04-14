using System;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Tasks;

namespace NewReleases
{
    public class ServerEntryPoint : IServerEntryPoint
    {
        private ILibraryManager LibraryManager { get; set; }
        private ITaskManager TaskManager       { get; set; }

        public ServerEntryPoint(ILibraryManager libraryManager, ITaskManager taskManager)
        {
            LibraryManager = libraryManager;
            TaskManager    = taskManager;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            Plugin.Instance.UpdateConfiguration(Plugin.Instance.Configuration);
            LibraryManager.ItemAdded += LibraryManager_ItemAdded;
        }

        private void LibraryManager_ItemAdded(object sender, ItemChangeEventArgs e)
        {
            foreach (var t in TaskManager.ScheduledTasks)
            {
                if (t.Name == "Refresh Internet Channels")
                {
                    TaskManager.Execute(t, new TaskOptions());
                }
            }
        }
    }
}
