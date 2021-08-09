using InputRecordReplay.InputHooks;
using InputRecordReplay.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using static InputRecordReplay.InputHooks.Win32;

namespace InputRecordReplay
{
    public class Player
    {
        public event Action<string> OnInput;
        public event Action OnStateChanged;
        public event Action<string> OnLog;
        private bool _isRecording = false;
        public bool IsRecording { get { return _isRecording; } private set { _isRecording = value; OnStateChanged?.Invoke(); } }
        private bool _isPlaying = false;
        public bool IsPlaying { get { return _isPlaying; } private set { _isPlaying = value; OnStateChanged?.Invoke(); } }
        public Stopwatch _stopwatch;

        private List<PlaybackRecord> playbackRecords;
        private MouseHook _mouseHook;
        private KeyboardHook _keyboardHook;
        private CancellationTokenSource _playbackCancel;
        private uint _handle;

        public Player(uint handle)
        {
            _handle = handle;
            _mouseHook = new MouseHook();
            _mouseHook.Install();
            _mouseHook.OnMouseInput += _mouseHook_OnMouseInput;
            _keyboardHook = new KeyboardHook();
            _keyboardHook.Install();
            _keyboardHook.OnKeyboardInput += _keyboardHook_OnKeyboardInput;
            _stopwatch = new Stopwatch();
        }

        private void _keyboardHook_OnKeyboardInput(INPUT obj)
        {
            if (IsRecording)
            {
                playbackRecords.Add(new PlaybackRecord()
                {
                    when = _stopwatch.Elapsed,
                    input = obj,
                });
            }
        }

        private void _mouseHook_OnMouseInput(INPUT obj)
        {
            if (IsRecording)
            {
                playbackRecords.Add(new PlaybackRecord()
                {
                    when = _stopwatch.Elapsed,
                    input = obj
                });
            }
        }

        //private void _mouseHook_OnMousePacket(int arg1, uint arg2, uint arg3)
        //{
        //    if (IsRecording)
        //    {
        //        playbackRecords.Add(new PlaybackRecord()
        //        {
        //            msg = (uint)arg1,
        //            wParam = arg2,
        //            lParam = arg3,
        //        });
        //    }
        //}

        //private void _keyboardHook_KeyDown(VKeys key)
        //{
        //    OnInput?.Invoke(key.ToString());
        //    //if (IsRecording)
        //    //    playbackRecords.Add(new PlaybackRecord()
        //    //    {
        //    //        type = 'k',
        //    //        when = _stopwatch.Elapsed,
        //    //    });
        //}

        //private void _mouseHook_MouseMove(MSLLHOOKSTRUCT mouseStruct)
        //{
        //    OnInput?.Invoke($"{mouseStruct.pt.x}, {mouseStruct.pt.y}");
        //    //if (IsRecording)
        //    //    playbackRecords.Add(new PlaybackRecord()
        //    //    {
        //    //        type = 'm',
        //    //        when = _stopwatch.Elapsed,
        //    //        dwFlags = mouseStruct.flags,
        //    //        dx = mouseStruct.pt.x,
        //    //        dy = mouseStruct.pt.y,
        //    //        dwData = mouseStruct.mouseData,
        //    //        dwExtraInfo = (int)mouseStruct.dwExtraInfo,
        //    //    });
        //}

        public void StartRecording()
        {
            playbackRecords = new List<PlaybackRecord>();
            _stopwatch.Restart();
            IsRecording = true;
        }

        public void StopRecording()
        {
            IsRecording = false;
            _stopwatch.Stop();
            OnLog?.Invoke("Recording has " + playbackRecords.Count + " entries. Over " + _stopwatch.Elapsed.TotalSeconds + " seconds.");
            // TODO: save playbackRecords
        }

        public void StartPlayback()
        {
            IsPlaying = true;
            _stopwatch.Restart();
            _playbackCancel = new CancellationTokenSource();
            Task.Run(async () =>
            {
                try
                {
                    int sizeOfINPUT = Marshal.SizeOf(typeof(INPUT));
                    OnLog?.Invoke("Starting playback...");
                    while (playbackRecords.Count > 0)
                    {
                        PlaybackRecord upNext = playbackRecords[0];
                        playbackRecords.RemoveAt(0);
                        TimeSpan howLongFromNow = upNext.when - _stopwatch.Elapsed;
                    if (howLongFromNow.Ticks > 0)
                        await Task.Delay((int)howLongFromNow.TotalMilliseconds);
                        //Thread.Sleep((int)howLongFromNow.TotalMilliseconds);
                        var result = SendInput(1, new INPUT[] { upNext.input }, sizeOfINPUT);
                        OnLog?.Invoke("sendinput: " + result);
                    //mouse_event(upNext.dwFlags, upNext.dx, upNext.dy, upNext.dwData, upNext.dwExtraInfo);
                    //SendMessage(_handle, upNext.wParam, upNext.msg, upNext.lParam);
                    }
                    OnLog?.Invoke("Completed playback...");
                    OnLog?.Invoke("playback records now has " + playbackRecords.Count + " entries.");
                }
                catch (Exception e)
                {
                    OnLog?.Invoke("Threw an exception in StartPlayback " + e.Message);
                }
                StopPlayback();
            }, _playbackCancel.Token);
        }

        public void StopPlayback()
        {
            _stopwatch.Stop();
            IsPlaying = false;
            _playbackCancel.Cancel();
        }

        public void Cleanup()
        {
            _mouseHook?.Uninstall();
            _keyboardHook?.Uninstall();
        }
    }
}
