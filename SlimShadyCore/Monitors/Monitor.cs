using SlimShadyCore.Utilities;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SlimShadyCore.Monitors
{
    public abstract class Monitor : NkComponentEntity<MonitorComponent>, IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event Utils.NkPropertyChangedEventHandler<string> DispNameChanged;
        protected void OnDispNameChanged(string newValue, [CallerMemberName] string propertyName = null)
        {
            DispNameChanged?.Invoke(propertyName, newValue);
            NotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Unique monitor identificator
        /// </summary>
        public readonly String Id;

        private string mDispName;
        /// <summary>
        /// Name of the monitor rendered to the user
        /// </summary>
        public string DispName
        {
            get => mDispName;
            set
            {
                mDispName = value;
                OnDispNameChanged(mDispName);
            }
        }

        protected Monitor(string id, string dispName)
        {
            Id = id;
            DispName = dispName;
        }

        public abstract void Dispose();

        public override string ToString()
        {
            return DispName + "(ID: " + Id + ")";
        }
    }
}
