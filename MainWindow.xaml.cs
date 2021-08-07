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
            DataContext = new MainWindowViewModel();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopup s = new SettingsPopup();
            s.ShowDialog();
        }
    }
}
