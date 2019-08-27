using SlimShadyEssentials.Monitors.Hardware.vcp;
using SlimShadyCore.Monitors.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using SlimShadyCore.Monitors;
using SlimShadyCore.Utilities;
using SlimShadyEssentials.MonitorManagers.Hardware.Components;
using SlimShadyEssentials.MonitorManagers.Hardware;
using SlimShadyCore.Utilities.Windows;

namespace SlimShadyEssentials.Monitors.Hardware
{
    public class HardwareMonitor : Monitor
    {
        private readonly PhysicalMonitor physicalMonitor;

        public HardwareMonitor(PhysicalMonitor physicalMonitor)
            : base(physicalMonitor.GetId(), physicalMonitor.GetName())
        {
            using (NkTrace trace = NkLogger.trace("id: " + Id + ", name: " + DispName))
            {
                this.physicalMonitor = physicalMonitor;
                InitializeBrightnessComponent();
                InitializeContrastComponent();
                InitializeTemperatureComponent();
                InitializeInputSourceComponent();
            }
        }

        private void InitializeBrightnessComponent()
        {
            try
            {
                PhysicalMonitor.Brightness pmBrightness = physicalMonitor.GetBrightness();
                AddComponent(new HardwareBrightnessComponent(physicalMonitor, pmBrightness.minBrightness, pmBrightness.maxBrightness, pmBrightness.brightness));
            }
            catch (Exception ex)
            {
                // Nothing. We expect for some monitors to not support this
                NkLogger.dbg("Failed to test brightness. Feature disabled. Ex: " + ex.Message);
            }
        }

        private void InitializeContrastComponent()
        {
            try
            {
                PhysicalMonitor.Contrast pmContrast = physicalMonitor.GetContrast();
                AddComponent(new HardwareContrastComponent(physicalMonitor, pmContrast.minContrast, pmContrast.maxContrast, pmContrast.contrast));
            }
            catch (Exception ex)
            {
                // Nothing. We expect for some monitors to not support this
                NkLogger.dbg("Failed to test contrast. Feature disabled. Ex: " + ex.Message);
            }
        }

        private void InitializeTemperatureComponent()
        {
            try
            {
                Win32MonitorCapabilities caps = physicalMonitor.GetCapabilities();
                List<uint> supportedTemps = caps.SupportedColorTemperatures;
                if (supportedTemps.Count <= 0)
                    return;

                uint pmTemperature = physicalMonitor.GetTemperature();
                AddComponent(new HardwareTemperatureComponent(physicalMonitor, supportedTemps.First(), supportedTemps.Last(), supportedTemps, pmTemperature));
            }
            catch (Exception ex)
            {
                // Nothing. We expect for some monitors to not support this
                NkLogger.dbg("Failed to test temperature. Feature disabled. Ex: " + ex.Message);
            }
        }

        private void InitializeInputSourceComponent()
        {
            if (!physicalMonitor.VcpCapabilities.SupportsInputSource())
                return;
            AddComponent(new HardwareInputSourceComponent(physicalMonitor));
        }

        public override void Dispose()
        {
            foreach (var component in GetComponents())
                component.Dispose();

            // hMonitors don't need to be cleaned.
        }
    }
}
