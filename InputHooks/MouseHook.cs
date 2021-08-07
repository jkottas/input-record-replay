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
    public class MouseHook
    {
        /// <summary>
        /// Internal callback processing function
        /// </summary>
        private DeviceHookHandler _hookHandler;

        /// <summary>
        /// Function to be called when defined even occurs
        /// </summary>
        /// <param name="mouseStruct">MSLLHOOKSTRUCT mouse structure</param>
        public delegate void MouseHookCallback(MSLLHOOKSTRUCT mouseStruct);

        #region Events
        public event MouseHookCallback LeftButtonDown;
        public event MouseHookCallback LeftButtonUp;
        public event MouseHookCallback RightButtonDown;
        public event MouseHookCallback RightButtonUp;
        public event MouseHookCallback MouseMove;
        public event MouseHookCallback MouseWheel;
        public event MouseHookCallback DoubleClick;
        public event MouseHookCallback MiddleButtonDown;
        public event MouseHookCallback MiddleButtonUp;
        #endregion

        /// <summary>
        /// Low level mouse hook's ID
        /// </summary>
        private IntPtr hookID = IntPtr.Zero;

        /// <summary>
        /// Install low level mouse hook
        /// </summary>
        /// <param name="mouseHookCallbackFunc">Callback function</param>
        public void Install()
        {
            _hookHandler = HookFunc;
            hookID = SetHook(_hookHandler);
        }

        /// <summary>
        /// Remove low level mouse hook
        /// </summary>
        public void Uninstall()
        {
            if (hookID == IntPtr.Zero)
                return;

            _ = UnhookWindowsHookEx(hookID);
            hookID = IntPtr.Zero;
        }

        /// <summary>
        /// Sets hook and assigns its ID for tracking
        /// </summary>
        private IntPtr SetHook(DeviceHookHandler proc)
        {
            using ProcessModule module = Process.GetCurrentProcess().MainModule;
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(module.ModuleName), 0);
        }

        /// <summary>
        /// Callback function
        /// </summary>
        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // parse system messages
            if (nCode >= 0)
            {
                MouseMessages mouseMessage = (MouseMessages)wParam;
                switch (mouseMessage)
                {
                    case MouseMessages.WM_LBUTTONDOWN:
                        LeftButtonDown?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case MouseMessages.WM_LBUTTONUP:
                        LeftButtonUp?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case MouseMessages.WM_MOUSEMOVE:
                        MouseMove?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case MouseMessages.WM_MOUSEWHEEL:
                        MouseWheel?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case MouseMessages.WM_RBUTTONDOWN:
                        RightButtonDown?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case MouseMessages.WM_RBUTTONUP:
                        RightButtonUp?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case MouseMessages.WM_LBUTTONDBLCLK:
                        DoubleClick?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case MouseMessages.WM_MBUTTONDOWN:
                        MiddleButtonDown?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case MouseMessages.WM_MBUTTONUP:
                        MiddleButtonUp?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    default:
                        break;
                }
            }
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }
    }
}
