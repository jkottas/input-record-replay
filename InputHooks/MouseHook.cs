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

        public event Action<INPUT> OnMouseInput;

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

        public const int MouseEventMove = 0x01;
        public const int MouseEventLeftDown = 0x02;
        public const int MouseEventLeftUp = 0x04;
        public const int MouseEventRightDown = 0x08;
        public const int MouseEventRightUp = 0x10;
        public const int MouseEventMiddleDown = 0x20;
        public const int MouseEventMiddleUp = 0x40;
        public const int MouseEventXDown = 0x80;
        public const int MouseEventXUp = 0x100;
        public const int MouseEventFWheel = 0x0800;
        public const int MouseEventFHWheel = 0x1000;
        public const int MouseEventAbsolute = 0x8000;

        private double screenHeight = 0;
        private double screenWidth = 0;

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
            screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
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

        POINT TranslateXY(POINT original)
        {
            return new POINT()
            {
                x = (int)((original.x * 0xFFFF) / (screenWidth -1)),
                y = (int)((original.y * 0xFFFF) / (screenHeight -1)),
            };
        }

        /// <summary>
        /// Callback function
        /// </summary>
        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MSLLHOOKSTRUCT rawMouseData = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

            uint flags = MouseEventAbsolute | MouseEventMove;
            switch ((MouseMessages)wParam)
            {
                case MouseMessages.WM_LBUTTONDOWN:
                    flags = MouseEventLeftDown;
                    break;
                case MouseMessages.WM_LBUTTONUP:
                    flags = MouseEventLeftUp;
                    break;
                case MouseMessages.WM_RBUTTONDOWN:
                    flags = MouseEventRightDown;
                    break;
                case MouseMessages.WM_RBUTTONUP:
                    flags = MouseEventRightUp;
                    break;
                default:
                case MouseMessages.WM_MOUSEMOVE:
                    flags = MouseEventAbsolute | MouseEventMove;
                    break;
                case MouseMessages.WM_MOUSEWHEEL:
                    flags = MouseEventFWheel;
                    break;
            }
            INPUT mouseInput = new INPUT
            {
                Type = INPUT_MOUSE,
                Data =
                {
                    Mouse = new MSLLHOOKSTRUCT()
                    {
                        dwExtraInfo = rawMouseData.dwExtraInfo,
                        flags = flags,
                        mouseData = rawMouseData.mouseData,
                        pt = TranslateXY(rawMouseData.pt),
                        time = 0,
                    }
                }
            };
        
    



            OnMouseInput?.Invoke(mouseInput);
            // parse system messages
            //if (nCode >= 0)
            //{
            //    MouseMessages mouseMessage = (MouseMessages)wParam;
            //    switch (mouseMessage)
            //    {
            //        case MouseMessages.WM_LBUTTONDOWN:
            //            LeftButtonDown?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
            //            break;
            //        case MouseMessages.WM_LBUTTONUP:
            //            LeftButtonUp?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
            //            break;
            //        case MouseMessages.WM_MOUSEMOVE:
            //            MouseMove?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
            //            break;
            //        case MouseMessages.WM_MOUSEWHEEL:
            //            MouseWheel?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
            //            break;
            //        case MouseMessages.WM_RBUTTONDOWN:
            //            RightButtonDown?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
            //            break;
            //        case MouseMessages.WM_RBUTTONUP:
            //            RightButtonUp?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
            //            break;
            //        case MouseMessages.WM_LBUTTONDBLCLK:
            //            DoubleClick?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
            //            break;
            //        case MouseMessages.WM_MBUTTONDOWN:
            //            MiddleButtonDown?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
            //            break;
            //        case MouseMessages.WM_MBUTTONUP:
            //            MiddleButtonUp?.Invoke((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
            //            break;
            //        default:
            //            break;
            //    }
            //}
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }
    }
}
