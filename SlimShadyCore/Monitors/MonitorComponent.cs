using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SlimShadyCore.Monitors
{
    public abstract class MonitorComponent : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract void Dispose();
    }
}
