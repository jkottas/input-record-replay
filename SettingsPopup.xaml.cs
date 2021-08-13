using InputRecordReplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InputRecordReplay
{
    /// <summary>
    /// Interaction logic for SettingsPopup.xaml
    /// </summary>
    public partial class SettingsPopup : Window
    {
        public SettingsPopup()
        {
            InitializeComponent();
        }

        private void recordingBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).SettingsBoxSelected(SettingsBoxes.RecordingBegin);
        }

        private void recordingEndBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).SettingsBoxSelected(SettingsBoxes.RecordingEnd);
        }

        private void playbackBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).SettingsBoxSelected(SettingsBoxes.PlaybackBegin);
        }

        private void plabackEndBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).SettingsBoxSelected(SettingsBoxes.PlaybackEnd);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (DataContext as MainWindowViewModel).SettingsBoxSelected(SettingsBoxes.None);
        }

        private void LoadBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).SettingsBoxSelected(SettingsBoxes.Load);
        }
    }
}
