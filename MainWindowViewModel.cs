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

        public DelegateCommand RecordButtonCommand { get; private set; }
        public DelegateCommand PlaybackButtonCommand { get; private set; }

        private KeyBindings _keyBindings;

        public MainWindowViewModel()
        {
            RecordButtonText = "Record (r)";
            PlaybackButtonText = "Playback (p)";
            RecordButtonCommand = new DelegateCommand(RecordButtonExecute, RecordButtonCanExecute);
            PlaybackButtonCommand = new DelegateCommand(PlaybackButtonExecute, PlaybackButtonCanExecute);
            _keyBindings = RestoreUserKeybindings();
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
    }
}
