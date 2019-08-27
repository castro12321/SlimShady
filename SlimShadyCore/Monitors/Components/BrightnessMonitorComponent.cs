using System.Runtime.CompilerServices;

namespace SlimShadyCore.Monitors.Components
{
    public abstract class BrightnessMonitorComponent : MonitorComponent
    {
        public event Utils.NkPropertyChangedEventHandler<uint> ValueChanged;
        protected void OnValueChanged(uint newValue, [CallerMemberName] string propertyName = null)
        {
            ValueChanged?.Invoke(propertyName, newValue);
            NotifyPropertyChanged(propertyName);
        }

        public abstract uint MinValue { get; }
        public abstract uint MaxValue { get; }
        public abstract uint CurrValue { get; set; }
        public void SetCurrValue(uint value) => CurrValue = value;
    }
}
