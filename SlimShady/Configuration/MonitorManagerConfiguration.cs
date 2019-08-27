using SlimShadyCore;
using SlimShadyCore.Monitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace SlimShady.Configuration
{
    public class MonitorManagerConfiguration
    {
        public event Utils.NkPropertyChangedEventHandler<object> PropertyChanged;
        protected void OnPropertyChanged(object newValue, [CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(propertyName, newValue);

        [XmlElement("Id")]
        public string Id { get; set; }

        private bool mEnabled = true;
        [XmlElement("Enabled")]
        public bool Enabled
        {
            get => mEnabled;
            set
            {
                mEnabled = value;
                OnPropertyChanged(value);
            }
        }

        [XmlElement("Monitor")]
        public List<MonitorConfiguration> Monitors { get; set; } = new List<MonitorConfiguration>();

        public void InitializeAfterDeserialization()
        {
            foreach (var mc in Monitors)
                mc.PropertyChanged += (property, val) => OnPropertyChanged(Monitors, nameof(Monitors));
        }

        public MonitorManagerConfiguration(string id)
        {
            this.Id = id;
        }

        [Obsolete] // For serialization only
        public MonitorManagerConfiguration()
        {

        }

        public MonitorConfiguration GetOrCreateMonitor(Monitor monitor) => GetOrCreateMonitor(monitor.Id);
        public MonitorConfiguration GetOrCreateMonitor(string id)
        {
            MonitorConfiguration mc = Monitors
                .Where(m => m.Id == id)
                .FirstOrDefault();
            if (mc == null)
            {
                mc = new MonitorConfiguration(id);
                mc.PropertyChanged += (property, val) => OnPropertyChanged(Monitors, nameof(Monitors));
                Monitors.Add(mc);
            }
            return mc;
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
