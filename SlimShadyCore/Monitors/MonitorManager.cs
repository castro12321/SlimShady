using SlimShadyCore.Utilities;
using SlimShadyCore.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SlimShadyCore.Monitors
{
    public abstract class MonitorManager : IDisposable
    {
        public string Id { get; private set; } 
        public string DispName { get; private set; }
        public INkObservableCollection<Monitor> Monitors { get; private set; } = NkObservableCollection<Monitor>.Create();

        protected abstract List<Monitor> ReadMonitors();

        public MonitorManager(string Id, string DispName)
        {
            this.Id = Id;
            this.DispName = DispName;
        }

        public virtual void RefreshMonitors()
        {
            using (NkTrace trace = NkLogger.trace("ID: " + Id))
            {

                DisposeMonitors();
                Monitors.Add(new EmptyMonitor("Loading..."));

                IEnumerable<Monitor> newMonitors = ReadMonitors();

                Monitors.Clear();
                if (newMonitors.Count() > 0)
                {
                    foreach (Monitor newMonitor in newMonitors)
                        Monitors.Add(newMonitor);
                }
                else
                {
                    Monitors.Add(new EmptyMonitor("Not supported..."));
                }

            }
        }

        public override string ToString()
        {
            return DispName + "(ID: " + Id + ")";
        }

        protected void DisposeMonitors()
        {
            foreach (Monitor m in Monitors)
                m.Dispose();
            Monitors.Clear();
        }

        public virtual void Dispose()
        {
            DisposeMonitors();
        }
    }
}
