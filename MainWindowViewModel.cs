using InputRecordReplay.InputHooks;
using InputRecordReplay.Models;
using InputRecordReplay.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
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

        public string RecordButtonText { get { return _player?.IsRecording??false?$"Stop Record({RecordButtonEndKey})":$"Record({RecordButtonBeginKey})"; } }
        public string PlaybackButtonText { get { return _player?.IsPlaying ?? false ? $"Stop Playback({PlaybackButtonEndKey})" : $"Play({PlaybackButtonBeginKey})"; } }

        private string _recentKeypress = "";
        public string RecentKeypress { get { return _recentKeypress; } set { _recentKeypress = value; Notify(); } }
        private string _recentMouse = "";
        public string RecentMouse { get { return _recentMouse; } set { _recentMouse = value; Notify(); } }

        public string RecordButtonBeginKey { get { return ConvertKeyName(_keyBindings?.RecordBeginButton); } }
        public string RecordButtonEndKey { get { return ConvertKeyName(_keyBindings?.RecordEndButton); } }
        public string PlaybackButtonBeginKey { get { return ConvertKeyName(_keyBindings?.PlaybackBeginButton); } }
        public string PlaybackButtonEndKey { get { return ConvertKeyName(_keyBindings?.PlaybackEndButton); } }

        public DelegateCommand RecordButtonCommand { get; private set; }
        public DelegateCommand PlaybackButtonCommand { get; private set; }

        private List<string> _logMessages = new List<string>();
        public string LogMessages { get { return string.Join("\n", _logMessages); } }

        private KeyBindings _keyBindings;
        private KeyBindings KeyBindings
        {
            get { return _keyBindings; }
            set
            {
                _keyBindings = value;
                Notify(nameof(RecordButtonBeginKey));
                Notify(nameof(RecordButtonEndKey));
                Notify(nameof(PlaybackButtonBeginKey));
                Notify(nameof(PlaybackButtonEndKey));
                Notify(nameof(RecordButtonText));
                Notify(nameof(PlaybackButtonText));
            }
        }

        private SettingsBoxes _selectedSettingsBox = SettingsBoxes.None;

        private Player _player;

        public MainWindowViewModel()
        {
            KeyBindings = RestoreUserKeybindings();            
            RecordButtonCommand = new DelegateCommand(RecordButtonExecute, RecordButtonCanExecute);
            PlaybackButtonCommand = new DelegateCommand(PlaybackButtonExecute, PlaybackButtonCanExecute);
            _player = new Player();
            _player.OnInput += _player_OnInput;
            _player.OnLog += _player_OnLog;
            _player.OnStateChanged += _player_OnStateChanged;
            _player.KeyboardKeyDown += _player_KeyboardKeyDown;
        }

        private void _player_OnStateChanged()
        {
            try
            {
                Notify(nameof(RecordButtonText));
                Notify(nameof(PlaybackButtonText));
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
            Notify(nameof(LogMessages));
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
            string previousBindings = Properties.Settings.Default.KeyBindings;
            if (string.IsNullOrEmpty(previousBindings))
                return new KeyBindings();
            else
                return JsonSerializer.Deserialize<KeyBindings>(previousBindings);
        }

        public void SaveUserKeybindings()
        {
            Properties.Settings.Default.KeyBindings = JsonSerializer.Serialize(KeyBindings);
            Properties.Settings.Default.Save();
            Notify(nameof(PlaybackButtonText));
            Notify(nameof(RecordButtonText));
        }

        public void SettingsBoxSelected(SettingsBoxes box)
        {
            _selectedSettingsBox = box;
        }

        private void _player_KeyboardKeyDown(string obj)
        {
            switch (_selectedSettingsBox)
            {
                case SettingsBoxes.None:
                default:
                    // do nothing
                    break;
                case SettingsBoxes.RecordingBegin:
                    _keyBindings.RecordBeginButton = obj;
                    Notify(nameof(RecordButtonBeginKey));
                    break;
                case SettingsBoxes.RecordingEnd:
                    _keyBindings.RecordEndButton = obj;
                    Notify(nameof(RecordButtonEndKey));
                    break;
                case SettingsBoxes.PlaybackBegin:
                    _keyBindings.PlaybackBeginButton = obj;
                    Notify(nameof(PlaybackButtonBeginKey));
                    break;
                case SettingsBoxes.PlaybackEnd:
                    _keyBindings.PlaybackEndButton = obj;
                    Notify(nameof(PlaybackButtonEndKey));
                    break;
            }
        }

        /// <summary>
        /// Takes a raw key string from the Win32.VKeys enum and converts it to a simple key. 
        /// For instance, "KEY_S" becomes "S"
        /// </summary>
        /// <param name="rawKey"></param>
        /// <returns></returns>
        public string ConvertKeyName(string rawKey)
        {
            return rawKey.Replace("KEY_", "");
        }

        public void Cleanup()
        {
            _player?.Cleanup();
        }
    }
}
