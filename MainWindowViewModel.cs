﻿using InputRecordReplay.InputHooks;
using InputRecordReplay.Models;
using InputRecordReplay.MVVM;
using Microsoft.Win32;
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

        public string RecordButtonText { get { return $"Record({RecordButtonBeginKey})"; } }
        public string RecordStopButtonText { get { return $"Stop Record({RecordButtonEndKey})"; } }
        public string PlaybackButtonText { get { return $"Play({PlaybackButtonBeginKey})"; } }
        public string PlaybackStopButtonText { get { return $"Stop Playback({PlaybackButtonEndKey})"; } }
        public string LoadButtonText { get { return $"Load({LoadButtonKey})"; } }

        private string _currentPlaybackElapsed = "0";
        public string CurrentPlaybackElapsed { get { return _currentPlaybackElapsed; } set { _currentPlaybackElapsed = value; Notify(); } }
        private string _maxPlaybackElapsed = "100";
        public string MaxPlaybackElapsed { get { return _maxPlaybackElapsed; } set { _maxPlaybackElapsed = value; Notify(); } }

        private string _recentKeypress = "";
        public string RecentKeypress { get { return _recentKeypress; } set { _recentKeypress = value; Notify(); } }
        private string _recentMouse = "";
        public string RecentMouse { get { return _recentMouse; } set { _recentMouse = value; Notify(); } }

        public string RecordButtonBeginKey { get { return ConvertKeyName(_keyBindings?.RecordBeginButton); } }
        public string RecordButtonEndKey { get { return ConvertKeyName(_keyBindings?.RecordEndButton); } }
        public string PlaybackButtonBeginKey { get { return ConvertKeyName(_keyBindings?.PlaybackBeginButton); } }
        public string PlaybackButtonEndKey { get { return ConvertKeyName(_keyBindings?.PlaybackEndButton); } }
        public string LoadButtonKey { get { return ConvertKeyName(_keyBindings?.LoadButton); } }

        public DelegateCommand RecordButtonCommand { get; private set; }
        public DelegateCommand RecordStopButtonCommand { get; private set; }
        public DelegateCommand PlaybackButtonCommand { get; private set; }
        public DelegateCommand PlaybackStopButtonCommand { get; private set; }
        public DelegateCommand LoadButtonCommand { get; private set; }

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
                Notify(nameof(LoadButtonText));
            }
        }

        private SettingsBoxes _selectedSettingsBox = SettingsBoxes.None;

        private Player _player;

        public MainWindowViewModel()
        {
            KeyBindings = RestoreUserKeybindings();            
            RecordButtonCommand = new DelegateCommand(RecordButtonExecute, RecordButtonCanExecute);
            RecordStopButtonCommand = new DelegateCommand(RecordStopButtonExecute, RecordStopButtonCanExecute);
            PlaybackButtonCommand = new DelegateCommand(PlaybackButtonExecute, PlaybackButtonCanExecute);
            PlaybackStopButtonCommand = new DelegateCommand(PlaybackStopButtonExecute, PlaybackStopButtonCanExecute);
            LoadButtonCommand = new DelegateCommand(LoadButtonExecute, LoadButtonCanExecute);
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
                RecordButtonCommand.RaiseCanExecuteChanged();
                RecordStopButtonCommand.RaiseCanExecuteChanged();
                PlaybackButtonCommand.RaiseCanExecuteChanged();
                PlaybackStopButtonCommand.RaiseCanExecuteChanged();
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
            return !_player.IsPlaying && !_player.IsRecording;
        }

        private void RecordButtonExecute(object parameter)
        {
            _player.StartRecording();
        }

        private bool RecordStopButtonCanExecute(object parameter)
        {
            return _player.IsRecording;
        }

        private void RecordStopButtonExecute(object parameter)
        {
            _player.StopRecording();
        }

        private bool PlaybackButtonCanExecute(object parameter)
        {
            return !_player.IsRecording && !_player.IsPlaying;
        }

        private void PlaybackButtonExecute(object parameter)
        {
            _player.StartPlayback();
        }
        private bool PlaybackStopButtonCanExecute(object parameter)
        {
            return _player.IsPlaying;
        }

        private void PlaybackStopButtonExecute(object parameter)
        {
            _player.StopPlayback();
        }

        private void LoadButtonExecute(object parameter)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                // load the recording
            }
        }

        private bool LoadButtonCanExecute(object parameter)
        {
            return !_player.IsPlaying && !_player.IsRecording;
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
            Notify(nameof(RecordButtonText));
            Notify(nameof(RecordStopButtonText));
            Notify(nameof(PlaybackButtonText));
            Notify(nameof(PlaybackStopButtonText));
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
                    if (obj == _keyBindings.RecordBeginButton && RecordButtonCanExecute(null))
                        RecordButtonExecute(null);
                    else if (obj == _keyBindings.RecordEndButton && RecordStopButtonCanExecute(null))
                        RecordStopButtonExecute(null);
                    else if (obj == _keyBindings.PlaybackBeginButton && PlaybackButtonCanExecute(null))
                        PlaybackButtonExecute(null);
                    else if (obj == _keyBindings.PlaybackEndButton && PlaybackStopButtonCanExecute(null))
                        PlaybackStopButtonExecute(null);
                    else if (obj == _keyBindings.LoadButton && LoadButtonCanExecute(null))
                        LoadButtonExecute(null);
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
