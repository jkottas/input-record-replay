using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static InputRecordReplay.InputHooks.Win32;

namespace InputRecordReplay.InputHooks
{
    public class KeyboardHook
    {
        private DeviceHookHandler _hookHandler;

        public delegate void KeyboardHookCallback(VKeys key);

        public event KeyboardHookCallback KeyDown;
        public event KeyboardHookCallback KeyUp;

        private IntPtr _hookID = IntPtr.Zero;

        public void Install()
        {
            _hookHandler = HookFunc;
            _hookID = SetHook(_hookHandler);
        }

        public void Uninstall()
        {
            _ = UnhookWindowsHookEx(_hookID);
        }

        /// <summary>
        /// Registers hook with Windows API
        /// </summary>
        private IntPtr SetHook(DeviceHookHandler proc)
        {
            using ProcessModule module = Process.GetCurrentProcess().MainModule;
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(module.ModuleName), 0);
        }

        /// <summary>
        /// Default hook call, which analyses pressed keys
        /// </summary>
        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int iwParam = wParam.ToInt32();
                switch (iwParam)
                {
                    case WM_KEYDOWN:
                    case WM_SYSKEYDOWN:
                        KeyDown?.Invoke((VKeys)Marshal.ReadInt32(lParam));
                        break;
                    case WM_KEYUP:
                    case WM_SYSKEYUP:
                        KeyUp?.Invoke((VKeys)Marshal.ReadInt32(lParam));
                        break;
                    default:
                        break;
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
