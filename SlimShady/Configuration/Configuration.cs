using SlimShadyCore;
using SlimShadyCore.Monitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace SlimShady.Configuration
{
    [XmlRoot("Configuration")]
    public class ConfigurationNew
    {
        public event Utils.NkPropertyChangedEventHandler<object> PropertyChanged;
        protected void OnPropertyChanged(object newValue, [CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(propertyName, newValue);

        //[XmlElement("EnabledDataFeeds")]
        //public List<string> EnabledDataFeeds { get; set; } = new List<string>();

        [XmlElement("MonitorManager")]
        public List<MonitorManagerConfiguration> MonitorManagers { get; set; } = new List<MonitorManagerConfiguration>();

        public void InitializeAfterDeserialization()
        {
            foreach (var mmc in MonitorManagers)
            {
                mmc.PropertyChanged += (property, newValue) => OnPropertyChanged(mmc, nameof(MonitorManagers));
                mmc.InitializeAfterDeserialization();
            }
        }

        [XmlElement("UI")]
        public UiConfiguration UI { get; set; } = new UiConfiguration();

        public bool StartWithWindows
        {
            get
            {
                return Utils.Autorun.Get("SlimShady");
            }
            set
            {
                Utils.Autorun.Set(Assembly.GetExecutingAssembly().Location, "SlimShady", value);
                OnPropertyChanged(value);
            }
        }

        public ConfigurationNew()
        {
            UI.PropertyChanged += (property, newValue) => OnPropertyChanged(UI, nameof(UI));
        }

        public MonitorManagerConfiguration GetOrCreateManager(MonitorManager manager) => GetOrCreateManager(manager.Id);
        public MonitorManagerConfiguration GetOrCreateManager(string id)
        {
            MonitorManagerConfiguration mmc = MonitorManagers
                .Where(mm => mm.Id == id)
                .FirstOrDefault();
            if (mmc == null)
            {
                mmc = new MonitorManagerConfiguration(id);
                mmc.PropertyChanged += (property, newValue) => OnPropertyChanged(mmc, nameof(MonitorManagers));
                MonitorManagers.Add(mmc);
            }
            return mmc;
        }
    }
}
