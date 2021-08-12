﻿using System;
using System.Runtime.InteropServices;

namespace InputRecordReplay.InputHooks
{
    public class Win32
    {
        public delegate IntPtr DeviceHookHandler(int nCode, IntPtr wParam, IntPtr lParam);
        public const int WM_KEYDOWN = 0x100;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYUP = 0x105;
        public const int WH_KEYBOARD_LL = 13;
        public const int WH_MOUSE_LL = 14;

        public const uint INPUT_MOUSE = 0;
        public const uint INPUT_KEYBOARD = 1;
        public const uint INPUT_HARDWARE = 2;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, DeviceHookHandler lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(uint hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MOUSEKEYBDHARDWAREINPUT
    {
        [FieldOffset(0)]
        public MSLLHOOKSTRUCT Mouse;

        [FieldOffset(0)]
        public KBDHOOKSTRUCT Keyboard;

        [FieldOffset(0)]
        public HARDWAREINPUT Hardware;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        /// <summary>
        /// INPUT_MOUSE, INPUT_KEYBOARD, or INPUT_HARDWARE
        /// </summary>
        public uint Type;
        public MOUSEKEYBDHARDWAREINPUT Data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KBDHOOKSTRUCT
    {
        public ushort KeyCode;
        public ushort Scan;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public uint Msg;
        public ushort ParamL;
        public ushort ParamH;
    }

    public enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208
    }

    public enum VKeys
    {
        // Losely based on http://www.pinvoke.net/default.aspx/Enums/VK.html

        LBUTTON = 0x01,     // Left mouse button
        RBUTTON = 0x02,     // Right mouse button
        CANCEL = 0x03,      // Control-break processing
        MBUTTON = 0x04,     // Middle mouse button (three-button mouse)
        XBUTTON1 = 0x05,    // Windows 2000/XP: X1 mouse button
        XBUTTON2 = 0x06,    // Windows 2000/XP: X2 mouse button
                            //                  0x07   // Undefined
        BACK = 0x08,        // BACKSPACE key
        TAB = 0x09,         // TAB key
                            //                  0x0A-0x0B,  // Reserved
        CLEAR = 0x0C,       // CLEAR key
        RETURN = 0x0D,      // ENTER key
                            //                  0x0E-0x0F, // Undefined
        SHIFT = 0x10,       // SHIFT key
        CONTROL = 0x11,     // CTRL key
        MENU = 0x12,        // ALT key
        PAUSE = 0x13,       // PAUSE key
        CAPITAL = 0x14,     // CAPS LOCK key
        KANA = 0x15,        // Input Method Editor (IME) Kana mode
        HANGUL = 0x15,      // IME Hangul mode
                            //                  0x16,  // Undefined
        JUNJA = 0x17,       // IME Junja mode
        FINAL = 0x18,       // IME final mode
        HANJA = 0x19,       // IME Hanja mode
        KANJI = 0x19,       // IME Kanji mode
                            //                  0x1A,  // Undefined
        ESCAPE = 0x1B,      // ESC key
        CONVERT = 0x1C,     // IME convert
        NONCONVERT = 0x1D,  // IME nonconvert
        ACCEPT = 0x1E,      // IME accept
        MODECHANGE = 0x1F,  // IME mode change request
        SPACE = 0x20,       // SPACEBAR
        PG_UP = 0x21,       // PAGE UP key
        PG_DN = 0x22,        // PAGE DOWN key
        END = 0x23,         // END key
        HOME = 0x24,        // HOME key
        LEFT = 0x25,        // LEFT ARROW key
        UP = 0x26,          // UP ARROW key
        RIGHT = 0x27,       // RIGHT ARROW key
        DOWN = 0x28,        // DOWN ARROW key
        SELECT = 0x29,      // SELECT key
        PRINT = 0x2A,       // PRINT key
        EXECUTE = 0x2B,     // EXECUTE key
        SNAPSHOT = 0x2C,    // PRINT SCREEN key
        INSERT = 0x2D,      // INS key
        DELETE = 0x2E,      // DEL key
        HELP = 0x2F,        // HELP key
        KEY_0 = 0x30,       // 0 key
        KEY_1 = 0x31,       // 1 key
        KEY_2 = 0x32,       // 2 key
        KEY_3 = 0x33,       // 3 key
        KEY_4 = 0x34,       // 4 key
        KEY_5 = 0x35,       // 5 key
        KEY_6 = 0x36,       // 6 key
        KEY_7 = 0x37,       // 7 key
        KEY_8 = 0x38,       // 8 key
        KEY_9 = 0x39,       // 9 key
                            //                  0x3A-0x40, // Undefined
        KEY_A = 0x41,       // A key
        KEY_B = 0x42,       // B key
        KEY_C = 0x43,       // C key
        KEY_D = 0x44,       // D key
        KEY_E = 0x45,       // E key
        KEY_F = 0x46,       // F key
        KEY_G = 0x47,       // G key
        KEY_H = 0x48,       // H key
        KEY_I = 0x49,       // I key
        KEY_J = 0x4A,       // J key
        KEY_K = 0x4B,       // K key
        KEY_L = 0x4C,       // L key
        KEY_M = 0x4D,       // M key
        KEY_N = 0x4E,       // N key
        KEY_O = 0x4F,       // O key
        KEY_P = 0x50,       // P key
        KEY_Q = 0x51,       // Q key
        KEY_R = 0x52,       // R key
        KEY_S = 0x53,       // S key
        KEY_T = 0x54,       // T key
        KEY_U = 0x55,       // U key
        KEY_V = 0x56,       // V key
        KEY_W = 0x57,       // W key
        KEY_X = 0x58,       // X key
        KEY_Y = 0x59,       // Y key
        KEY_Z = 0x5A,       // Z key
        LWIN = 0x5B,        // Left Windows key (Microsoft Natural keyboard)
        RWIN = 0x5C,        // Right Windows key (Natural keyboard)
        APPS = 0x5D,        // Applications key (Natural keyboard)
                            //                  0x5E, // Reserved
        SLEEP = 0x5F,       // Computer Sleep key
        NUMPAD0 = 0x60,     // Numeric keypad 0 key
        NUMPAD1 = 0x61,     // Numeric keypad 1 key
        NUMPAD2 = 0x62,     // Numeric keypad 2 key
        NUMPAD3 = 0x63,     // Numeric keypad 3 key
        NUMPAD4 = 0x64,     // Numeric keypad 4 key
        NUMPAD5 = 0x65,     // Numeric keypad 5 key
        NUMPAD6 = 0x66,     // Numeric keypad 6 key
        NUMPAD7 = 0x67,     // Numeric keypad 7 key
        NUMPAD8 = 0x68,     // Numeric keypad 8 key
        NUMPAD9 = 0x69,     // Numeric keypad 9 key
        MULTIPLY = 0x6A,    // Multiply key
        ADD = 0x6B,         // Add key
        SEPARATOR = 0x6C,   // Separator key
        SUBTRACT = 0x6D,    // Subtract key
        DECIMAL = 0x6E,     // Decimal key
        DIVIDE = 0x6F,      // Divide key
        F1 = 0x70,          // F1 key
        F2 = 0x71,          // F2 key
        F3 = 0x72,          // F3 key
        F4 = 0x73,          // F4 key
        F5 = 0x74,          // F5 key
        F6 = 0x75,          // F6 key
        F7 = 0x76,          // F7 key
        F8 = 0x77,          // F8 key
        F9 = 0x78,          // F9 key
        F10 = 0x79,         // F10 key
        F11 = 0x7A,         // F11 key
        F12 = 0x7B,         // F12 key
        F13 = 0x7C,         // F13 key
        F14 = 0x7D,         // F14 key
        F15 = 0x7E,         // F15 key
        F16 = 0x7F,         // F16 key
        F17 = 0x80,         // F17 key  
        F18 = 0x81,         // F18 key  
        F19 = 0x82,         // F19 key  
        F20 = 0x83,         // F20 key  
        F21 = 0x84,         // F21 key  
        F22 = 0x85,         // F22 key, (PPC only) Key used to lock device.
        F23 = 0x86,         // F23 key  
        F24 = 0x87,         // F24 key  
                            //                  0x88-0X8F,  // Unassigned
        NUMLOCK = 0x90,     // NUM LOCK key
        SCROLL = 0x91,      // SCROLL LOCK key
                            //                  0x92-0x96,  // OEM specific
                            //                  0x97-0x9F,  // Unassigned
        LSHIFT = 0xA0,      // Left SHIFT key
        RSHIFT = 0xA1,      // Right SHIFT key
        LCONTROL = 0xA2,    // Left CONTROL key
        RCONTROL = 0xA3,    // Right CONTROL key
        LMENU = 0xA4,       // Left MENU key
        RMENU = 0xA5,       // Right MENU key
        BROWSER_BACK = 0xA6,    // Windows 2000/XP: Browser Back key
        BROWSER_FORWARD = 0xA7, // Windows 2000/XP: Browser Forward key
        BROWSER_REFRESH = 0xA8, // Windows 2000/XP: Browser Refresh key
        BROWSER_STOP = 0xA9,    // Windows 2000/XP: Browser Stop key
        BROWSER_SEARCH = 0xAA,  // Windows 2000/XP: Browser Search key
        BROWSER_FAVORITES = 0xAB,  // Windows 2000/XP: Browser Favorites key
        BROWSER_HOME = 0xAC,    // Windows 2000/XP: Browser Start and Home key
        VOLUME_MUTE = 0xAD,     // Windows 2000/XP: Volume Mute key
        VOLUME_DOWN = 0xAE,     // Windows 2000/XP: Volume Down key
        VOLUME_UP = 0xAF,  // Windows 2000/XP: Volume Up key
        MEDIA_NEXT_TRACK = 0xB0,// Windows 2000/XP: Next Track key
        MEDIA_PREV_TRACK = 0xB1,// Windows 2000/XP: Previous Track key
        MEDIA_STOP = 0xB2, // Windows 2000/XP: Stop Media key
        MEDIA_PLAY_PAUSE = 0xB3,// Windows 2000/XP: Play/Pause Media key
        LAUNCH_MAIL = 0xB4,     // Windows 2000/XP: Start Mail key
        LAUNCH_MEDIA_SELECT = 0xB5,  // Windows 2000/XP: Select Media key
        LAUNCH_APP1 = 0xB6,     // Windows 2000/XP: Start Application 1 key
        LAUNCH_APP2 = 0xB7,     // Windows 2000/XP: Start Application 2 key
                                //                  0xB8-0xB9,  // Reserved
        SEMICOLON = 0xBA,       // Used for miscellaneous characters; it can vary by keyboard.
                            // Windows 2000/XP: For the US standard keyboard, the ';:' key
        PLUS = 0xBB,    // Windows 2000/XP: For any country/region, the '+' key
        COMMA = 0xBC,   // Windows 2000/XP: For any country/region, the ',' key
        MINUS = 0xBD,   // Windows 2000/XP: For any country/region, the '-' key
        PERIOD = 0xBE,  // Windows 2000/XP: For any country/region, the '.' key
        FWDSLASH = 0xBF,       // Used for miscellaneous characters; it can vary by keyboard.
                            // Windows 2000/XP: For the US standard keyboard, the '/?' key
        TILDE = 0xC0,       // Used for miscellaneous characters; it can vary by keyboard.
                            // Windows 2000/XP: For the US standard keyboard, the '`~' key
                            //                  0xC1-0xD7,  // Reserved
                            //                  0xD8-0xDA,  // Unassigned
        OPENBRACE = 0xDB,       // Used for miscellaneous characters; it can vary by keyboard.
                            // Windows 2000/XP: For the US standard keyboard, the '[{' key
        BACKSLASH = 0xDC,       // Used for miscellaneous characters; it can vary by keyboard.
                            // Windows 2000/XP: For the US standard keyboard, the '\|' key
        CLOSEBRACE = 0xDD,       // Used for miscellaneous characters; it can vary by keyboard.
                            // Windows 2000/XP: For the US standard keyboard, the ']}' key
        QUOTE = 0xDE,       // Used for miscellaneous characters; it can vary by keyboard.
                            // Windows 2000/XP: For the US standard keyboard, the 'single-quote/double-quote' key
        OEM_8 = 0xDF,       // Used for miscellaneous characters; it can vary by keyboard.
                            //                  0xE0,  // Reserved
                            //                  0xE1,  // OEM specific
        OEM_102 = 0xE2,     // Windows 2000/XP: Either the angle bracket key or the backslash key on the RT 102-key keyboard
                            //                  0xE3-E4,  // OEM specific
        PROCESSKEY = 0xE5,  // Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
                            //                  0xE6,  // OEM specific
        PACKET = 0xE7,      // Windows 2000/XP: Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
                            //                  0xE8,  // Unassigned
                            //                  0xE9-F5,  // OEM specific
        ATTN = 0xF6,        // Attn key
        CRSEL = 0xF7,       // CrSel key
        EXSEL = 0xF8,       // ExSel key
        EREOF = 0xF9,       // Erase EOF key
        PLAY = 0xFA,        // Play key
        ZOOM = 0xFB,        // Zoom key
        NONAME = 0xFC,      // Reserved
        PA1 = 0xFD,         // PA1 key
        OEM_CLEAR = 0xFE    // Clear key
    }
}
