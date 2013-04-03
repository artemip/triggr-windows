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

namespace triggr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static NotifyIcon icon;
        private bool _closing = false;

        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);
            InitializeComponent();
            DataContext = TriggrViewModel.Model;

            MainWindow.icon = new NotifyIcon();

            System.Windows.Forms.MenuItem[] menuItems = {
                new System.Windows.Forms.MenuItem("Show", showWindow_Handler),
                new System.Windows.Forms.MenuItem("Exit", exit_Handler)
            };

            var ctxMenu = new System.Windows.Forms.ContextMenu(menuItems);

            icon.Click += new EventHandler(showWindow_Handler);
            icon.ContextMenu = ctxMenu;
            var iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Images/favicon.ico")).Stream;
            icon.Icon = new System.Drawing.Icon(iconStream);
            icon.Visible = true;

            if (Properties.Settings.Default.LastLaunch == null || DateTime.Now - Properties.Settings.Default.LastLaunch > TimeSpan.FromDays(30))
            {
                this.Show();
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
