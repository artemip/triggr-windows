using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace Triggr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        private MainWindow _win;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Disallow multiple Triggr processes from running at the same time
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
                ShowWindow(runningProcess.MainWindowHandle, 5);
                this.Shutdown();
                return;
            }            

            base.OnStartup(e);

            _win = new MainWindow();
        }
    }
}
