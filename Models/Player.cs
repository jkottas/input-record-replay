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
        public event Action<string> KeyboardKeyDown;
        public event Action PlaybackComplete; // fires once each time the playback completes.
        private bool _isRecording = false;
        public bool IsRecording { get { return _isRecording; } private set { _isRecording = value; OnStateChanged?.Invoke(); } }
        private bool _isPlaying = false;
        public bool IsPlaying { get { return _isPlaying; } private set { _isPlaying = value; OnStateChanged?.Invoke(); } }
        public Stopwatch _stopwatch;

        public List<PlaybackRecord> PlaybackRecords;
        private MouseHook _mouseHook;
        private KeyboardHook _keyboardHook;
        private CancellationTokenSource _playbackCancel;

        public Player()
        {
            _mouseHook = new MouseHook();
            _mouseHook.Install();
            _mouseHook.OnMouseInput += _mouseHook_OnMouseInput;
            _keyboardHook = new KeyboardHook();
            _keyboardHook.Install();
            _keyboardHook.OnKeyboardInput += _keyboardHook_OnKeyboardInput;
            _keyboardHook.KeyDown += _keyboardHook_KeyDown;
            _stopwatch = new Stopwatch();
        }

        private void _keyboardHook_KeyDown(VKeys key)
        {
            KeyboardKeyDown?.Invoke(key.ToString());
        }

        private void _keyboardHook_OnKeyboardInput(INPUT obj)
        {
            if (IsRecording)
            {
                PlaybackRecords.Add(new PlaybackRecord()
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
                PlaybackRecords.Add(new PlaybackRecord()
                {
                    when = _stopwatch.Elapsed,
                    input = obj
                });
            }
        }

        public void StartRecording()
        {
            PlaybackRecords = new List<PlaybackRecord>();
            _stopwatch.Restart();
            IsRecording = true;
        }

        public void StopRecording()
        {
            IsRecording = false;
            _stopwatch.Stop();
            PlaybackRecords.RemoveRange(PlaybackRecords.Count - 5, 5); // last entry will be the mouse click or keyboard press that ended the recording. We don't want that.
            OnLog?.Invoke("Recording has " + PlaybackRecords.Count + " entries. Over " + _stopwatch.Elapsed.TotalSeconds + " seconds.");
        }

        public void StartPlayback()
        {
            IsPlaying = true;
            _playbackCancel = new CancellationTokenSource();
            Task.Run(async () =>
            {
                try
                {
                    while (!_playbackCancel.IsCancellationRequested) // play forever
                    {
                        List<PlaybackRecord> playbackRecordsCopy = new List<PlaybackRecord>(PlaybackRecords);
                        int sizeOfINPUT = Marshal.SizeOf(typeof(INPUT));
                        OnLog?.Invoke("Starting playback...");
                        _stopwatch.Restart();
                        while (!_playbackCancel.IsCancellationRequested && playbackRecordsCopy.Count > 0)
                        {
                            PlaybackRecord upNext = playbackRecordsCopy[0];
                            playbackRecordsCopy.RemoveAt(0);
                            TimeSpan howLongFromNow = upNext.when - _stopwatch.Elapsed;
                            if (howLongFromNow.Ticks > 0)
                                await Task.Delay((int)howLongFromNow.TotalMilliseconds);
                            var result = SendInput(1, new INPUT[] { upNext.input }, sizeOfINPUT);
                        }
                        PlaybackComplete?.Invoke();
                    }
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
            _playbackCancel.Cancel();
            _stopwatch.Stop();
            IsPlaying = false;
        }

        public void Cleanup()
        {
            _mouseHook?.Uninstall();
            _keyboardHook?.Uninstall();
        }
    }
}
