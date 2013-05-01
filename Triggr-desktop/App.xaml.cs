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
        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);
        
        protected override void OnStartup(StartupEventArgs e)
        {
            Process currentProcess = Process.GetCurrentProcess();
            var runningProcess = (from process in Process.GetProcesses()
                                  where
                                    process.Id != currentProcess.Id &&
                                    process.ProcessName.Equals(
                                      currentProcess.ProcessName,
                                      StringComparison.Ordinal)
                                  select process).FirstOrDefault();
            if (runningProcess != null)
            {
                ShowWindow(runningProcess.MainWindowHandle, SW_SHOWMAXIMIZED);
                return; 
            }            

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
