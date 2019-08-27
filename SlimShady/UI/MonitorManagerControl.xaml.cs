using SlimShady.Configuration;
using SlimShadyCore.Monitors;
using SlimShadyCore.Utilities;
using SlimShadyCore.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace SlimShady.UI
{
    /// <summary>
    /// Interaction logic for MonitorManagerControl.xaml
    /// </summary>
    public partial class MonitorManagerControl : UserControl
    {
        public class MonitorWithConfig
        {
            public Monitor Monitor { get; private set; }
            public MonitorConfiguration Config { get; private set; }

            public MonitorWithConfig(Monitor monitor, MonitorConfiguration config)
            {
                Monitor = monitor;
                Config = config;
            }
        }

        private class MonitorOrderComparer : IComparer<Monitor>
        {
            private readonly MonitorManagerConfiguration managerConfig;

            public MonitorOrderComparer(MonitorManagerConfiguration managerConfig)
            {
                this.managerConfig = managerConfig;
            }

            public int Compare(Monitor monitor1, Monitor monitor2)
            {
                int order1 = managerConfig.GetOrCreateMonitor(monitor1).Order ?? 1;
                int order2 = managerConfig.GetOrCreateMonitor(monitor2).Order ?? 1;
                return order1.CompareTo(order2);
            }
        }

        public IObservableEnumerable<MonitorWithConfig> Monitors { get; private set; }
        private object monitorsLock = new object();

        public MonitorManagerControl(MonitorManager manager, MonitorManagerConfiguration managerConfig)
        {
            MonitorWithConfig mapper(Monitor monitor) => new MonitorWithConfig(monitor, managerConfig.GetOrCreateMonitor(monitor));

            var sorted = new SortedObservableCollection<Monitor>(manager.Monitors, new MonitorOrderComparer(managerConfig));
            var mapped = new MappedObservableCollection<Monitor, MonitorWithConfig>(sorted, mapper);
            Monitors = mapped;
            BindingOperations.EnableCollectionSynchronization(Monitors, monitorsLock);
            
            DataContext = this;
            InitializeComponent();
        }
    }
}
