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
                        if (config.IncrementMonths) {
                            view.querySelector('#enableMonthlyIncrements').checked = config.IncrementMonths;
                        } else {
                            view.querySelector('#enableMonthlyIncrements').checked = false;
                        }

                        if (config.IncrementDays) {
                            view.querySelector('#enableDailyIncrements').checked = config.IncrementDays;
                        } else {
                            view.querySelector('#enableDailyIncrements').checked = false;
                        }

                    });

                    view.querySelector('#enableMonthlyIncrements').addEventListener('change',
                        () => {

                            if (view.querySelector('#enableMonthlyIncrements').checked === true) {
                                view.querySelector('#enableDailyIncrements').checked = false;
                            } 

                            ApiClient.getPluginConfiguration(pluginId).then((config) => {
                                config.IncrementMonths = view.querySelector('#enableMonthlyIncrements').checked;
                                config.IncrementMonths = view.querySelector('#enableDailyIncrements').checked;
                                ApiClient.updatePluginConfiguration(pluginId, config).then(result => {
                                    Dashboard.processPluginConfigurationUpdateResult(result);
                                });
                            });
                        });

                    view.querySelector('#enableDailyIncrements').addEventListener('change',
                        () => {

                            if (view.querySelector('#enableDailyIncrements').checked === true) {
                                view.querySelector('#enableMonthlyIncrements').checked = false;
                            }

                            ApiClient.getPluginConfiguration(pluginId).then((config) => {
                                config.IncrementMonths = view.querySelector('#enableMonthlyIncrements').checked;
                                config.IncrementMonths = view.querySelector('#enableDailyIncrements').checked;
                                ApiClient.updatePluginConfiguration(pluginId, config).then(result => {
                                    Dashboard.processPluginConfigurationUpdateResult(result);
                                });
                            });
                        });

                    var minPremiereDateTargetValue;
                    view.querySelector('#txtMinDateOffSet').addEventListener('change', (e) => {
                        minPremiereDateTargetValue = e.target.value;
                    });

                    view.querySelector('#saveData').addEventListener('click',
                        () => {
                            ApiClient.getPluginConfiguration(pluginId).then((config) => {
                                config.MinPremiereDate = minPremiereDateTargetValue;
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