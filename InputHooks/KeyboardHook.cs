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
        public event Action<INPUT> OnKeyboardInput;

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
                ushort key = (ushort)Marshal.ReadInt32(lParam);
                int action = wParam.ToInt32();
                uint flag = (uint)((action == WM_KEYUP || action == WM_SYSKEYUP) ? 0x0002 : 0x0000);
                INPUT rawKeyboardInput = new INPUT()
                {
                    Type = INPUT_KEYBOARD,
                    Data =
                    {
                        Keyboard = new KBDHOOKSTRUCT()
                        {
                            KeyCode = key,
                            Scan = 0,
                            Flags = flag,
                            Time = 0,
                            ExtraInfo = IntPtr.Zero,
                        }
                    }
                };
                OnKeyboardInput?.Invoke(rawKeyboardInput);
                switch (action)
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
