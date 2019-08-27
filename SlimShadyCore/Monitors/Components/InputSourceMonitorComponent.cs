using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SlimShadyCore.Monitors.Components
{
    public abstract class InputSourceMonitorComponent : MonitorComponent
    {
        public event Utils.NkPropertyChangedEventHandler<string> ValueChanged;
        protected void OnValueChanged(string newValue, [CallerMemberName] string propertyName = null)
        {
            ValueChanged?.Invoke(propertyName, newValue);
            NotifyPropertyChanged(propertyName);
        }

        public abstract List<string> SupportedInputsources { get; }
        public abstract string ActiveInputSource { get; set; }
        public void SetActiveInputSource(string value) => ActiveInputSource = value;
    }
}
