using SlimShady.Configuration;
using SlimShadyCore;
using SlimShadyCore.Monitors;
using SlimShadyCore.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SlimShady.UI.Configuration
{
    public partial class MonitorManagerConfigControl : UserControl
    {
        public class SupportedMonitorManagerModel
        {
            private readonly MonitorManagerProvider provider;
            private readonly ConfigurationNew config = ConfigurationHelper.GetConfig();
            public string DispName => provider.DispName;
            public bool Enabled
            {
                get => config.GetOrCreateManager(provider.Id).Enabled;
                set => config.GetOrCreateManager(provider.Id).Enabled = value;
            }

            public SupportedMonitorManagerModel(MonitorManagerProvider mmp)
            {
                this.provider = mmp;
            }
        }

        public class MonitorMgrCfgModel : DependencyObject, INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private IObservableEnumerable<SupportedMonitorManagerModel> mSupportedMonitorManagers = NkObservableCollection<SupportedMonitorManagerModel>.Create();
            public IObservableEnumerable<SupportedMonitorManagerModel> SupportedMonitorManagers
            {
                get => mSupportedMonitorManagers;
                set
                {
                    mSupportedMonitorManagers = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public MonitorMgrCfgModel Model { get; private set; } = new MonitorMgrCfgModel();

        public static readonly DependencyProperty CoreProperty = DependencyProperty.Register("Core", typeof(SlimCore), typeof(MonitorManagerConfigControl), new FrameworkPropertyMetadata(null, OnCoreChanged));
        public SlimCore Core
        {
            get => (SlimCore)GetValue(CoreProperty);
            set => SetValue(CoreProperty, value);
        }
        private static void OnCoreChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonitorManagerConfigControl mmcc = ((MonitorManagerConfigControl)d);
            SlimCore newCore = mmcc.Core;

            Func<MonitorManagerProvider, SupportedMonitorManagerModel> mapper = (provider) => new SupportedMonitorManagerModel(provider);
            INkObservableCollection<MonitorManagerProvider> p = NkObservableCollection<MonitorManagerProvider>.Create(newCore.SupportedMonitorManagers);
            var SupportedMonitorManagers = new MappedObservableCollection<MonitorManagerProvider, SupportedMonitorManagerModel>(newCore.SupportedMonitorManagers, mapper);

            mmcc.Model.SupportedMonitorManagers = SupportedMonitorManagers;
        }


        /*public static readonly DependencyProperty CoreProperty = DependencyProperty.Register("Core", typeof(SlimCore), typeof(MonitorManagerConfigControl));
        public SlimCore Core
        {
            get => (SlimCore)GetValue(CoreProperty);
            set
            {
                SetValue(CoreProperty, value);

                Func<MonitorManagerProvider, SupportedMonitorManagerModel> mapper = (provider) => new SupportedMonitorManagerModel(provider);
                INkObservableCollection<MonitorManagerProvider> p = NkObservableCollection<MonitorManagerProvider>.Create(value.SupportedMonitorManagers);
                Model.SupportedMonitorManagers = new MappedObservableCollection<MonitorManagerProvider, SupportedMonitorManagerModel>(value.SupportedMonitorManagers, mapper);
            }
        }*/

        public MonitorManagerConfigControl()
        {
            DataContext = Model;
            InitializeComponent();
        }
    }
}
