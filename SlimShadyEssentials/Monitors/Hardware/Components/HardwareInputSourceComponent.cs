using SlimShadyCore.Monitors.Components;
using SlimShadyCore.Utilities;
using SlimShadyEssentials.Monitors.Hardware;
using SlimShadyEssentials.Monitors.Hardware.vcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace SlimShadyEssentials.MonitorManagers.Hardware.Components
{
    public class HardwareInputSourceComponent : InputSourceMonitorComponent
    {
        private readonly Timer refreshTimer;
        private readonly PhysicalMonitor physicalMonitor;
        private readonly List<string> supportedInputSources;
        private string activeInputSource;

        public override List<string> SupportedInputsources => supportedInputSources;
        public override string ActiveInputSource
        {
            get => activeInputSource;
            set
            {
                if (activeInputSource == value)
                {
                    NkLogger.dbg("Change not needed, already at: " + value);
                    return;
                }

                using (NkTrace trace = NkLogger.trace("val: " + value))
                {
                    InputSource inputSource = (InputSource)Enum.Parse(typeof(InputSource), value);
                    physicalMonitor.SetInputSource(inputSource);
                    activeInputSource = value;
                    OnValueChanged(activeInputSource);
                }
            }
        }

        public HardwareInputSourceComponent(PhysicalMonitor physicalMonitor)
        {
            this.physicalMonitor = physicalMonitor;

            supportedInputSources = physicalMonitor.VcpCapabilities
                .GetSupportedInputSources()
                .Select(inputSource => inputSource.ToString())
                .ToList();
            activeInputSource = getRefreshActiveInputSource().ToString();

            refreshTimer = new Timer(RefreshTimer_Tick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        private InputSource getRefreshActiveInputSource()
        {
            return physicalMonitor.GetInputSource();
        }

        private void RefreshTimer_Tick(object arg)
        {
            InputSource newInputSourceRaw = getRefreshActiveInputSource();
            string newInputSource = newInputSourceRaw.ToString();
            if (newInputSource == activeInputSource || newInputSourceRaw == InputSource.Unknown)
                return;
            NkLogger.info("Detected ActiveInputSource change! " + activeInputSource + " -> " + newInputSource);
            activeInputSource = newInputSource;
            OnValueChanged(activeInputSource, nameof(ActiveInputSource));
        }

        override public void Dispose()
        {
            refreshTimer.Dispose();
        }
    }
}
