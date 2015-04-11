using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Controls;

namespace SlimShady
{
    [SuppressMessage("ReSharper", "InconsistentNaming")] // Names are copied from WinApi
    public class ShortcutsWinApi
    {
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_KEYUP = 0x0101;
        public const int WM_SYSKEYUP = 0x0105;

        public delegate int LowLevelKeyboardHookFunctionCallback(int Code, int wParam, ref keyBoardHookStruct lParam);
        public struct keyBoardHookStruct { public int vkCode, scanCode, flags, time, dwExtraInfo; }

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, int wParam, ref keyBoardHookStruct lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardHookFunctionCallback lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    }

    // TODO:
    // - lctrl + 1/2/3/n --> profile 1/2/3/n

    // DONE:
    // These use default Manager
    // - lAlt + pg up/down --> master Brightness +/-10%
    // - lAlt + home/end --> master Contrast +/-10%
    // - lAlt + 1/2/3/n + pg up/down --> monitor 1/2/3/n Brightness +/-10%
    // - lAlt + 1/2/3/n + home/end --> monitor 1/2/3/n Contrast +/-10%
    public class Shortcuts
    {
        private void KeyDown(KeyEventArgs args)
        {
            pressedKeys.Add(args.KeyCode);

            if(pressedKeys.Contains(Keys.LControlKey))
            {
                // Handle profile changing 1/2/3/n
            }

            if(pressedKeys.Contains(Keys.LMenu)) // left alt
            {
                if (args.KeyCode == Keys.PageUp || args.KeyCode == Keys.PageDown
                || args.KeyCode == Keys.Home || args.KeyCode == Keys.End)
                {
                    int monitorNum = 0;
                    if (pressedKeys.Contains(Keys.D1)) monitorNum = 1; // Sorry
                    else if (pressedKeys.Contains(Keys.D2)) monitorNum = 2; // Sorry for that too
                    else if (pressedKeys.Contains(Keys.D3)) monitorNum = 3; // And for that
                    else if (pressedKeys.Contains(Keys.D4)) monitorNum = 4; // And that
                    else if (pressedKeys.Contains(Keys.D5)) monitorNum = 5; // I'm sooooo sorry
                    else if (pressedKeys.Contains(Keys.D6)) monitorNum = 6; // ;(
                    else if (pressedKeys.Contains(Keys.D7)) monitorNum = 7; // HAHA
                    else if (pressedKeys.Contains(Keys.D8)) monitorNum = 8; // I lied
                    else if (pressedKeys.Contains(Keys.D9)) monitorNum = 9; // I'm perfectly fine with all these
                    
                    StackPanel panel = (StackPanel)MainWindow.Instance.MonitorManagerTabControl.SelectedContent;
                    MonitorSliders s = (MonitorSliders)panel.Children[monitorNum];
                    switch(args.KeyCode)
                    {
                        case Keys.PageUp:
                            //s.BrightnessSlider.Value = (int)(s.BrightnessSlider.Value * 1.05) + 1;
                            s.BrightnessSlider.Value += 5;
                            break;
                        case Keys.PageDown:
                            //s.BrightnessSlider.Value = (int)(s.BrightnessSlider.Value * 0.95) - 1;
                            s.BrightnessSlider.Value -= 5;
                            break;
                        case Keys.Home:
                            //s.ContrastSlider.Value = (int)(s.ContrastSlider.Value * 1.05) + 1;
                            s.ContrastSlider.Value += 5;
                            break;
                        case Keys.End:
                            //s.ContrastSlider.Value = (int)(s.ContrastSlider.Value * 0.95) - 1;
                            s.ContrastSlider.Value -= 5;
                            break;
                    }
                }
            }
        }

        private void KeyUp(KeyEventArgs args)
        {
            pressedKeys.Remove(args.KeyCode);
        }

        public Shortcuts()
        {
            callback = LowLevelKeyboardProc;
            hookProcedureHandle = ShortcutsWinApi.SetWindowsHookEx(ShortcutsWinApi.WH_KEYBOARD_LL, callback, IntPtr.Zero, 0);
        }

        ~Shortcuts()
        {
            ShortcutsWinApi.UnhookWindowsHookEx(hookProcedureHandle);
        }

        private readonly IntPtr hookProcedureHandle;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable - Otherwise it would be garbage collected. Use AllocGC()?!
        private readonly ShortcutsWinApi.LowLevelKeyboardHookFunctionCallback callback;
        private readonly List<Keys> pressedKeys = new List<Keys>();

        private int LowLevelKeyboardProc(int nCode, int wParam, ref ShortcutsWinApi.keyBoardHookStruct lParam)
        {
            if (nCode >= 0)
            {
                KeyEventArgs eventArg = new KeyEventArgs((Keys)lParam.vkCode);
                switch(wParam)
                {
                    case ShortcutsWinApi.WM_KEYDOWN:
                    case ShortcutsWinApi.WM_SYSKEYDOWN:
                        KeyDown(eventArg);
                        break;
                    case ShortcutsWinApi.WM_KEYUP:
                    case ShortcutsWinApi.WM_SYSKEYUP:
                        KeyUp(eventArg);
                        break;
                }

                if (eventArg.Handled)
                    return 1;
            }
            return ShortcutsWinApi.CallNextHookEx(IntPtr.Zero, nCode, wParam, ref lParam);
        }
    }
}
