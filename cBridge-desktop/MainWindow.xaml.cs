using System;
using System.Collections.Generic;
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

namespace cBridge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private float oldVolume;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void handleCallEvent(string data)
        {
            if(data == "incoming_call")
            {
                oldVolume = VolumeControl.getCurrentVolume();
                VolumeControl.setVolume(0.1F);
            } else if(data == "call_ended")
            {
                VolumeControl.setVolume(oldVolume);
            }
        }
    }
}
