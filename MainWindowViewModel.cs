using InputRecordReplay.InputHooks;
using InputRecordReplay.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InputRecordReplay
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _recordButtonText = "";
        public string RecordButtonText { get { return _recordButtonText; } set { _recordButtonText = value; Notify(); } }
        private string _playbackButtonText = "";
        public string PlaybackButtonText { get { return _playbackButtonText; } set { _playbackButtonText = value; Notify(); } }
        private string _recentKeypress = "";
        public string RecentKeypress { get { return _recentKeypress; } set { _recentKeypress = value; Notify(); } }
        private string _recentMouse = "";
        public string RecentMouse { get { return _recentMouse; } set { _recentMouse = value; Notify(); } }

        private string _recordButtonBeginKey = "";
        public string RecordButtonBeginKey { get { return _recordButtonBeginKey; } set { _recordButtonBeginKey = value; Notify(); } }
        private string _recordButtonEndKey = "";
        public string RecordButtonEndKey { get { return _recordButtonEndKey; } set { _recordButtonEndKey = value; Notify(); } }
        private string _playbackButtonBeginKey = "";
        public string PlaybackButtonBeginKey { get { return _playbackButtonBeginKey; } set { _playbackButtonBeginKey = value; Notify(); } }
        private string _playbackButtonEndKey = "";
        public string PlaybackButtonEndKey { get { return _playbackButtonEndKey; } set { _playbackButtonEndKey = value; Notify(); } }

        public DelegateCommand RecordButtonCommand { get; private set; }
        public DelegateCommand PlaybackButtonCommand { get; private set; }

        private List<string> _logMessages = new List<string>();
        public string LogMessages { get { return string.Join("\n", _logMessages); } }

        private KeyBindings _keyBindings;
        private Player _player;

        public MainWindowViewModel()
        {
            RecordButtonText = "Record (r)";
            PlaybackButtonText = "Playback (p)";
            RecordButtonCommand = new DelegateCommand(RecordButtonExecute, RecordButtonCanExecute);
            PlaybackButtonCommand = new DelegateCommand(PlaybackButtonExecute, PlaybackButtonCanExecute);
            _keyBindings = RestoreUserKeybindings();
            _player = new Player();
            _player.OnInput += _player_OnInput;
            _player.OnLog += _player_OnLog;
            _player.OnStateChanged += _player_OnStateChanged;
        }

        private void _player_OnStateChanged()
        {
            try
            {
                if (_player.IsRecording)
                    RecordButtonText = "Stop Recording (s)";
                else
                    RecordButtonText = "Start Recording (r)";
                if (_player.IsPlaying)
                    PlaybackButtonText = "Stop (s)";
                else
                    PlaybackButtonText = "Play (p)";
                RecordButtonCommand.RaiseCanExecuteChanged();
                PlaybackButtonCommand.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                Log("Threw exception in _player_OnStateChanged " + e.Message);
            }
        }

        private void _player_OnLog(string obj)
        {
            Log(obj);
        }

        private void Log(string message)
        {
            _logMessages.Add(message);
            if (_logMessages.Count > 40)
                _logMessages.RemoveAt(0);
            Notify("LogMessages");
        }

        private void _player_OnInput(string whatHappened)
        {
            RecentKeypress = whatHappened;
        }

        private bool RecordButtonCanExecute(object parameter)
        {
            return !_player.IsPlaying;
        }

        private void RecordButtonExecute(object parameter)
        {
            if (_player.IsRecording)
                _player.StopRecording();
            else
                _player.StartRecording();
        }

        private bool PlaybackButtonCanExecute(object parameter)
        {
            return !_player.IsRecording;
        }

        private void PlaybackButtonExecute(object parameter)
        {
            if (_player.IsPlaying)
                _player.StopPlayback();
            else
                _player.StartPlayback();
        }

        private KeyBindings RestoreUserKeybindings()
        {
            return new KeyBindings();
        }

        public void Cleanup()
        {
            _player?.Cleanup();
        }
    }
}
