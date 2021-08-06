using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InputRecordReplay
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string RecordButtonText { get; set; }
        public string PlaybackButtonText { get; set; }

        public DelegateCommand RecordButtonClick { get; private set; }
        public DelegateCommand PlaybackButtonClick { get; private set; }
        public DelegateCommand SettingsButtonClick;

        public MainWindowViewModel()
        {
            RecordButtonText = "Record (r)";
            PlaybackButtonText = "Playback (p)";
            RecordButtonClick = new DelegateCommand(RecordButtonExecute, RecordButtonCanExecute);
            PlaybackButtonClick = new DelegateCommand(PlaybackButtonExecute, PlaybackButtonCanExecute);
            SettingsButtonClick = new DelegateCommand(PerformSettingsButtonClick);
        }

        private bool RecordButtonCanExecute()
        {
            return true;
        }

        private void RecordButtonExecute()
        {
            RecordButtonText = "Stop Recording (s)";
        }

        private bool PlaybackButtonCanExecute()
        {
            return true;
        }

        private void PlaybackButtonExecute()
        {
        }

        private void PerformSettingsButtonClick()
        {
        }
    }
}
