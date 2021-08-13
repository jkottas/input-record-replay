using InputRecordReplay.InputHooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InputRecordReplay.Models
{
    public class SerializablePlaybackRecord
    {
        public uint Type { get; set; }
        public MKHStruct mkhStruct { get; set; }
        public double timeSpanMilliseconds { get; set; }

        public SerializablePlaybackRecord()
        {
        }

        public SerializablePlaybackRecord(PlaybackRecord source)
        {
            timeSpanMilliseconds = source.when.TotalMilliseconds;
            Type = source.input.Type;
            mkhStruct = new MKHStruct(source.input.Data, Type);
        }

        public static List<SerializablePlaybackRecord> MakeSerializable(List<PlaybackRecord> playbackRecords)
        {
            return playbackRecords.Select(x => new SerializablePlaybackRecord(x)).ToList();
        }

        public static List<PlaybackRecord> MakeUseable(List<SerializablePlaybackRecord> serializablePlaybackRecords)
        {
            List<PlaybackRecord> returnMe = new List<PlaybackRecord>();
            foreach (SerializablePlaybackRecord record in serializablePlaybackRecords)
            {
                if (record.Type == Win32.INPUT_KEYBOARD)
                {
                    PlaybackRecord pr = new PlaybackRecord()
                    {
                        when = TimeSpan.FromMilliseconds(record.timeSpanMilliseconds),
                        input = new INPUT
                        {
                            Type = record.Type,
                            Data =
                            {
                                Keyboard = new KBDHOOKSTRUCT()
                                {
                                    KeyCode = record.mkhStruct.keyStruct.KeyCode,
                                    Scan = 0,
                                    Flags = record.mkhStruct.keyStruct.Flags,
                                    Time = 0,
                                    ExtraInfo = IntPtr.Zero,
                                }
                            }
                        }
                    };
                    returnMe.Add(pr);
                }
                else if (record.Type == Win32.INPUT_MOUSE)
                {
                    PlaybackRecord pr = new PlaybackRecord()
                    {
                        when = TimeSpan.FromMilliseconds(record.timeSpanMilliseconds),
                        input = new INPUT
                        {
                            Type = record.Type,
                            Data =
                            {
                                Mouse = new MSLLHOOKSTRUCT()
                                {
                                    dwExtraInfo = (IntPtr)record.mkhStruct.mouseStruct.dwExtraInfo,
                                    flags = record.mkhStruct.mouseStruct.flags,
                                    mouseData = record.mkhStruct.mouseStruct.mouseData,
                                    pt = new POINT
                                    {
                                        x = (int)record.mkhStruct.mouseStruct.point.X,
                                        y = (int)record.mkhStruct.mouseStruct.point.Y,
                                    },
                                    time = 0,
                                }
                            }
                        }
                    };
                    returnMe.Add(pr);
                }
            }
            return returnMe;
        }
    }

    public class MKHStruct
    {
        public MouseStruct mouseStruct { get; set; }
        public KeyStruct keyStruct { get; set; }
        public HardwareStruct hardwareStruct { get; set; }

        public MKHStruct() { }
        public MKHStruct(MOUSEKEYBDHARDWAREINPUT source, uint type)
        {
            if (type == Win32.INPUT_MOUSE)
                mouseStruct = new MouseStruct(source.Mouse);
            else if (type == Win32.INPUT_KEYBOARD)
                keyStruct = new KeyStruct(source.Keyboard);
            else if (type == Win32.INPUT_HARDWARE)
                hardwareStruct = new HardwareStruct(source.Hardware);
        }
    }

    public class MouseStruct
    {
        public Point point { get; set; }
        public uint mouseData { get; set; }
        public uint flags { get; set; }
        public int dwExtraInfo { get; set; }

        public MouseStruct() { }
        public MouseStruct(MSLLHOOKSTRUCT source)
        {
            point = new Point(source.pt.x, source.pt.y);
            mouseData = source.mouseData;
            flags = source.flags;
            dwExtraInfo = (int)source.dwExtraInfo;
        }
    }

    public class KeyStruct
    {
        public ushort KeyCode { get; set; }
        public ushort Scan { get; set; }
        public uint Flags { get; set; }
        public int ExtraInfo { get; set; }
        public KeyStruct() { }
        public KeyStruct(KBDHOOKSTRUCT source)
        {
            KeyCode = source.KeyCode;
            Scan = source.Scan;
            Flags = source.Flags;
            ExtraInfo = (int)source.ExtraInfo;
        }
    }

    public class HardwareStruct
    {
        public uint Msg { get; set; }
        public ushort ParamL { get; set; }
        public ushort ParamH { get; set; }
        public HardwareStruct() { }
        public HardwareStruct(HARDWAREINPUT source)
        {
            Msg = source.Msg;
            ParamL = source.ParamL;
            ParamH = source.ParamH;
        }
    }
}
