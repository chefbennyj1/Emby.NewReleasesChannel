define(["require", "loading", "dialogHelper", "formDialogStyle", "emby-checkbox", "emby-select", "emby-toggle"],
    function(require, loading, dialogHelper) {
        var pluginId = "33A1E636-0FDC-4EEA-8B3A-F2971274164B";

        ApiClient.refreshInternetChannels = function (id) {
            var url = this.getUrl("ScheduledTasks/Running/" + id);
            return this.ajax({
                type: "POST",
                url: url
            });
        };    

        return function(view) {
            view.addEventListener('viewshow',
                () => {
                    ApiClient.getPluginConfiguration(pluginId).then((config) => {
                        if (config.MinPremiereDate) {
                            view.querySelector('#txtMinDateOffSet').value = config.MinPremiereDate;
                        }
                    });

                    view.querySelector('#txtMinDateOffSet').addEventListener('change', (e) => {
                        ApiClient.getPluginConfiguration(pluginId).then((config) => {
                            config.MinPremiereDate = e.target.value;
                            ApiClient.updatePluginConfiguration(pluginId, config).then(
                                (result) => {

                                    ApiClient.getJSON(ApiClient.getUrl("ScheduledTasks")).then(tasks => {
                                        tasks.forEach(task => {
                                            if (task.Name === "Refresh Internet Channels") {

                                                ApiClient.refreshInternetChannels(task.Id);
                                                Dashboard.processPluginConfigurationUpdateResult(result);

                                            }
                                        });
                                    });

                                    
                                });
                        });
                    });
                });
        }
    });