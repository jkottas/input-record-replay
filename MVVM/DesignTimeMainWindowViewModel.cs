using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputRecordReplay.MVVM
{
    public class DesignTimeMainWindowViewModel
    {
        public string RecordButtonText { get { return $"Record(R)"; } }
        public string RecordStopButtonText { get { return $"Stop Record(S)"; } }
        public string PlaybackButtonText { get { return $"Play(P)"; } }
        public string PlaybackStopButtonText { get { return $"Stop Playback(S)"; } }
        public string LoadButtonText { get { return $"Load(L)"; } }
        public string CurrentPlaybackElapsed { get { return "45"; } }
        public string MaxPlaybackElapsed { get { return "100"; } }


        public DelegateCommand RecordButtonCommand { get; private set; }
        public DelegateCommand RecordStopButtonCommand { get; private set; }
        public DelegateCommand PlaybackButtonCommand { get; private set; }
        public DelegateCommand PlaybackStopButtonCommand { get; private set; }
        public DelegateCommand LoadButtonCommand { get; private set; }

        public DesignTimeMainWindowViewModel()
        {
            RecordButtonCommand = new DelegateCommand(null, CanExecute);
            RecordStopButtonCommand = new DelegateCommand(null, CannotExecute);
            PlaybackButtonCommand = new DelegateCommand(null, CanExecute);
            PlaybackStopButtonCommand = new DelegateCommand(null, CannotExecute);
            LoadButtonCommand = new DelegateCommand(null, CanExecute);
        }

        public bool CannotExecute(object parameter)
        {
            return false;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
