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
using System.IO;
using System.Net;
using NetSparkle;

namespace Triggr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static NotifyIcon icon;
        private Sparkle _sparkle;
        private bool _closing = false;

        public MainWindow()
        {
            // Handle global exceptions
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

            InitializeComponent();

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

            // Enable auto-updater
            _sparkle = new Sparkle("http://www.triggrapp.com/download/update/versioninfo2.xml", iconImage);
            //_sparkle.EnableSilentMode = true;
            _sparkle.StartLoop(true);

            // If application is new, open it
            if (Properties.Settings.Default.IsFirstLaunch || DateTime.Now - Properties.Settings.Default.LastLaunch > TimeSpan.FromDays(15))
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
                if (Properties.Settings.Default.IsFirstLaunch)
                {
                    icon.ShowBalloonTip(5000, "", "Triggr is running in the background", ToolTipIcon.Info);
                    Properties.Settings.Default.IsFirstLaunch = false;
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
