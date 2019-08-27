using SlimShady.Utilities;
using SlimShadyCore.Monitors;
using SlimShadyCore.Utilities;
using SlimShadyCore.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SlimShadyCore
{
    public class SlimCore
    {
        public INkObservableCollection<MonitorManagerProvider> SupportedMonitorManagers { get; private set; } = NkObservableCollection<MonitorManagerProvider>.Create();
        public INkObservableCollection<MonitorManager> ActiveMonitorManagers { get; private set; } = NkObservableCollection<MonitorManager>.Create();
        //public List<DataFeed> DataFeeds { get; private set; } = new List<DataFeed>();
        //public ConfigurationNew Config { get; private set; }

        public void AddSupportedMonitorManager(MonitorManagerProvider monitorManagerProvider)
        {
            SupportedMonitorManagers.Add(monitorManagerProvider);
        }

        public bool IsActive(MonitorManagerProvider provider)
        {
            return ActiveMonitorManagers.Any(mm => mm.Id == provider.Id);
        }

        public void ActivateMonitorManager(MonitorManagerProvider monitorManagerProvider)
        {
            if (IsActive(monitorManagerProvider))
                throw new Exception("MonitorManager("+monitorManagerProvider.Id+") is already active");
            ActiveMonitorManagers.Add(monitorManagerProvider.GetOrCreate());
        }
    }
}
