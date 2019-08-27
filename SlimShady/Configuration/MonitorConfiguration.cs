using SlimShadyCore;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace SlimShady.Configuration
{
    public class MonitorConfiguration
    {
        public event Utils.NkPropertyChangedEventHandler<object> PropertyChanged;
        protected void OnPropertyChanged(object newValue, [CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(propertyName, newValue);

        private string mId;
        [XmlElement("Id")]
        public string Id
        {
            get => mId;
            set
            {
                mId = value;
                OnPropertyChanged(value);
            }
        }

        private string mDispName;
        [XmlElement("DispName")]
        public string DispName
        {
            get => mDispName;
            set
            {
                mDispName = value;
                OnPropertyChanged(value);
            }
        }

        private int? mOrder;
        [XmlElement("Order")]
        public int? Order
        {
            get => mOrder;
            set
            {
                mOrder = value;
                OnPropertyChanged(value);
            }
        }
        public bool ShouldSerializeOrder() { return Order.HasValue; }

        private uint? mBrightness;
        [XmlElement("Brightness")]
        public uint? Brightness
        {
            get => mBrightness;
            set
            {
                mBrightness = value;
                OnPropertyChanged(value);
            }
        }
        public bool ShouldSerializeBrightness() { return Brightness.HasValue; }

        private uint? mContrast;
        [XmlElement("Contrast")]
        public uint? Contrast
        {
            get => mContrast;
            set
            {
                mContrast = value;
                OnPropertyChanged(value);
            }
        }
        public bool ShouldSerializeContrast() { return Contrast.HasValue; }

        private uint? mTemperature;
        [XmlElement("Temperature")]
        public uint? Temperature
        {
            get => mTemperature;
            set
            {
                mTemperature = value;
                OnPropertyChanged(value);
            }
        }
        public bool ShouldSerializeTemperature() { return Temperature.HasValue; }

        public MonitorConfiguration(string id)
        {
            this.Id = id;
        }

        [Obsolete] // For serialization only
        public MonitorConfiguration()
        {

        }

        public override string ToString()
        {
            return Id;
        }
    }
}
