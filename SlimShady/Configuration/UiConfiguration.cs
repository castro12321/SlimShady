using SlimShadyCore;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace SlimShady.Configuration
{
    public class UiConfiguration
    {
        public event Utils.NkPropertyChangedEventHandler<object> PropertyChanged;
        protected void OnPropertyChanged(object newValue, [CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(propertyName, newValue);

        private string mActiveMonitorManager;
        [XmlElement("ActiveMonitorManager")]
        public string ActiveMonitorManager
        {
            get => mActiveMonitorManager;
            set
            {
                mActiveMonitorManager = value;
                OnPropertyChanged(value);
            }
        }
    }
}
