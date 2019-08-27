using SlimShadyCore;
using SlimShadyCore.Utilities;
using SlimShadyCore.Utilities.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SlimShady.Utilities
{
    public class MonitorChangeDetector
    {
        private class McdForm : Form
        {
            private readonly Action<Message> onMessage;

            public McdForm(Action<Message> onMessage)
            {
                this.onMessage = onMessage;
            }

            protected override void WndProc(ref Message m)
            {
                onMessage(m);
                base.WndProc(ref m);
            }
        }

        public delegate void HardwareMonitorChangeDetectedHandler();
        public event HardwareMonitorChangeDetectedHandler HardwareMonitorChangeDetected;

        private readonly McdForm mcdForm;

        public MonitorChangeDetector()
        {
            mcdForm = new McdForm((m) => onMessage(m));
            mcdForm.Show();
            mcdForm.Visible = false;
            mcdForm.ShowInTaskbar = false;
        }

        private void onMessage(Message m)
        {
            if (m.Msg != Win.WM_DEVICECHANGE)
                return;

            int eventType = m.WParam.ToInt32();
            if(eventType != Win.DBT_DEVNODES_CHANGED)
            {
                NkLogger.info("IGNORING DEVICE_CHANGED EVENT WITH EV_TYPE: " + eventType.ToString("X"));
                return;
            }

            NkLogger.info("Monitor topology change detected; Refreshing; Screens: " + Screen.AllScreens.Length);
            HardwareMonitorChangeDetected?.Invoke();
        }

    }
}
