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
using CoreAudioApi;

namespace cBridge_desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MMDevice device;
        private float oldVolume;

        public MainWindow()
        {
            InitializeComponent();
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var socket = OpenSocketConnection("ec2-75-101-183-71.compute-1.amazonaws.com", 9090);

            if(socket == null)
            {
                addEventToList("Connection Failed.", eventsListBox);
                return;
            }

            addEventToList("Connected.", eventsListBox);

            while(socket.Connected)
            {
                Byte[] bytesReceived = new Byte[256];

                int bytes = 0;
                string data = "";

                do
                {
                    bytes = socket.Receive(bytesReceived, bytesReceived.Length, 0);
                    data = Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                   
                    handleCallEvent(data);
                }
                while (bytes > 0);
            }
        }

        private void setVolume(float desiredVolume)
        {
            int numSteps = 100;
            float initVolume = device.AudioEndpointVolume.MasterVolumeLevelScalar;
            float stepAmount = Math.Abs(initVolume - desiredVolume) / numSteps;

            bool lowerVolume = initVolume > desiredVolume;

            for (int i = 0; i < numSteps; ++i)
            {
                Thread.Sleep(5);
                if(lowerVolume)
                    device.AudioEndpointVolume.MasterVolumeLevelScalar -= stepAmount;
                else
                    device.AudioEndpointVolume.MasterVolumeLevelScalar += stepAmount;
            }
        }

        private void addEventToList(string eventItem, ListBox listBox)
        {
            eventsListBox.Items.Add(DateTime.Now.TimeOfDay + " - " + eventItem);
            
        }

        private void handleCallEvent(string data)
        {
            addEventToList(data, eventsListBox);
            if(data == "incoming_call")
            {
                oldVolume = device.AudioEndpointVolume.MasterVolumeLevelScalar;
                setVolume(0.1F);
            } else if(data == "call_ended")
            {
                setVolume(oldVolume);
            }
        }

        private Socket OpenSocketConnection(string server, int port)
        {
            string request = "GET / HTTP/1.1\r\nHost: " + server +
                "\r\nConnection: Close\r\n\r\n";
            Byte[] bytesSent = Encoding.ASCII.GetBytes(request);

            addEventToList("Connecting...", eventsListBox);
            
            // Create a socket connection with the specified server and port.
            Socket s = SocketHelper.ConnectSocket(server, port);

            if (s == null)
                return null;

            // Send request to the server.
            s.Send(bytesSent, bytesSent.Length, 0);

            return s;
        }
    }
}
