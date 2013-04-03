using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace triggr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new System.Threading.Mutex(true, "triggrapp");

            base.OnStartup(e);

            //Handle automatic startup
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", "Triggr", "\"" + Assembly.GetExecutingAssembly().Location + "\"", RegistryValueKind.String);

            //Open window
            var win = new MainWindow();
            win.Closed += win_Closed;
        }

        void win_Closed(object sender, EventArgs e)
        {
            mutex.ReleaseMutex();
            this.Shutdown();
        }
    }
}
