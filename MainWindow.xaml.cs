using System.Windows;
using System.Windows.Input;

namespace InputRecordReplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new MainWindowViewModel();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopup s = new SettingsPopup();
            s.DataContext = DataContext;
            _ = s.ShowDialog();
            (DataContext as MainWindowViewModel)?.SaveUserKeybindings();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (DataContext as MainWindowViewModel)?.Cleanup();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
