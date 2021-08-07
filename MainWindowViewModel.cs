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

        public DelegateCommand RecordButtonCommand { get; private set; }
        public DelegateCommand PlaybackButtonCommand { get; private set; }

        private KeyBindings _keyBindings;
        private MouseHook _mouseHook;
        private KeyboardHook _keyboardHook;

        public MainWindowViewModel()
        {
            RecordButtonText = "Record (r)";
            PlaybackButtonText = "Playback (p)";
            RecordButtonCommand = new DelegateCommand(RecordButtonExecute, RecordButtonCanExecute);
            PlaybackButtonCommand = new DelegateCommand(PlaybackButtonExecute, PlaybackButtonCanExecute);
            _keyBindings = RestoreUserKeybindings();
            _mouseHook = new MouseHook();
            _mouseHook.Install();
            _mouseHook.MouseMove += _mouseHook_MouseMove;
            _keyboardHook = new KeyboardHook();
            _keyboardHook.Install();
            _keyboardHook.KeyDown += _keyboardHook_KeyDown;
        }

        private void _keyboardHook_KeyDown(VKeys key)
        {
            RecentKeypress = key.ToString();
        }

        private void _mouseHook_MouseMove(MSLLHOOKSTRUCT mouseStruct)
        {
            RecentMouse = $"{mouseStruct.pt.x}, {mouseStruct.pt.y}";
        }

        private bool RecordButtonCanExecute(object parameter)
        {
            return true;
        }

        private void RecordButtonExecute(object parameter)
        {
            RecordButtonText = "Stop Recording (s)";
        }

        private bool PlaybackButtonCanExecute(object parameter)
        {
            return true;
        }

        private void PlaybackButtonExecute(object parameter)
        {
        }

        private KeyBindings RestoreUserKeybindings()
        {
            return new KeyBindings();
        }

        public void Cleanup()
        {
            _mouseHook?.Uninstall();
            _keyboardHook?.Uninstall();
        }
    }
}
