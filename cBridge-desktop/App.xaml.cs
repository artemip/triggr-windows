using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace cbridge
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Handle automatic startup
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", "cBridge", "\"" + Assembly.GetExecutingAssembly().Location + "\"", RegistryValueKind.String);

            //Open window
            var win = new MainWindow();
            win.Closed += win_Closed;     
        }

        void win_Closed(object sender, EventArgs e)
        {
            this.Shutdown();
        }
    }
}
