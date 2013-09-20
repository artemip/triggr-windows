using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Deployment.Application;
using Microsoft.Win32;
using System.IO;
using System.Net;
using System.Reflection;
using Triggr.ViewModels;

namespace Triggr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static NotifyIcon icon;
        private bool _closing = false;

        private static void SetAddRemoveProgramsIcon()
        {
            try
            {
                string iconSourcePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "triggr_icon.ico");
                if (!File.Exists(iconSourcePath))
                {
                    return;
                }

                RegistryKey myUninstallKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
                string[] mySubKeyNames = myUninstallKey.GetSubKeyNames();
                for (int i = 0; i < mySubKeyNames.Length; i++)
                {
                    RegistryKey myKey = myUninstallKey.OpenSubKey(mySubKeyNames[i], true);
                    object myValue = myKey.GetValue("DisplayName");
                    Console.WriteLine(myValue.ToString());
                    // Set this to the display name of the application. If you are not sure, browse to the registry directory and check.
                    if (myValue != null && myValue.ToString() == "Triggr")
                    {
                        myKey.SetValue("DisplayIcon", iconSourcePath);
                        break;
                    }
                }
            }
            catch(Exception e)
            {
                
            }
        }

        private static void SetRegistryKeys() {
            //Handle automatic startup
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", "Triggr", "\"" + Assembly.GetExecutingAssembly().Location + "\"", RegistryValueKind.String);
        }

        public MainWindow()
        {
            // Handle global exceptions
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

            InitializeComponent();

            if (true)//ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                SetRegistryKeys();
                SetAddRemoveProgramsIcon();
            }

            // Set Data Context (for data bindings, as per MVVM)
            DataContext = TriggrViewModel.Model;

            // Extract icon
            var iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Images/favicon.ico")).Stream;
            var iconImage = new System.Drawing.Icon(iconStream);

            // Set tray icon
            MainWindow.icon = new NotifyIcon();
            icon.Icon = iconImage;

            // Set tray icon menu items
            System.Windows.Forms.MenuItem[] menuItems = {
                new System.Windows.Forms.MenuItem("Show", showWindow_Handler),
                new System.Windows.Forms.MenuItem("Exit", exit_Handler)
            };
            var ctxMenu = new System.Windows.Forms.ContextMenu(menuItems);
            icon.Click += new EventHandler(showWindow_Handler);
            icon.BalloonTipClicked += new EventHandler(showWindow_Handler);
            icon.ContextMenu = ctxMenu;

            // Enable tray icon
            icon.Visible = true;

            // If application is new, open it
            if (ApplicationDeployment.CurrentDeployment.IsFirstRun || DateTime.Now - Properties.Settings.Default.LastLaunch > TimeSpan.FromDays(15))
            {
                this.Show();
            }
            else // Otherwise, show a balloon tooltip
            {
                icon.ShowBalloonTip(5000, "", "Triggr is running in the background", ToolTipIcon.Info);
            }

            Properties.Settings.Default.LastLaunch = DateTime.Now;
            Properties.Settings.Default.Save();
        }

        private void showWindow_Handler(Object sender, EventArgs e)
        {
            this.Show();
        }

        private void exit_Handler(Object sender, EventArgs e)
        {
            _closing = true;
            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!_closing)
            {
                e.Cancel = true;
                this.Hide();
                if (ApplicationDeployment.CurrentDeployment.IsFirstRun)
                {
                    icon.ShowBalloonTip(5000, "", "Triggr is running in the background", ToolTipIcon.Info);
                    Properties.Settings.Default.Save();
                }
            }
            else
            {
                icon.Visible = false;
                TriggrViewModel.Model.Dispose();
            }
        }

        private void HandleHeaderMouseDown(Object sender,
           MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //Do logging
        }

        private void InitiatePairing(object sender, RoutedEventArgs e)
        {
            TriggrViewModel.Model.PairingModeEnabled = true;
        }

        private void HandleCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
