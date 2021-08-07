using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InputRecordReplay
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static KeyBindings KeyBindings { get; set;}

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            
            // save keybindings to settings
        }
    }
}
