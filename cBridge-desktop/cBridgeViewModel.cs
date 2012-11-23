﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace cBridge
{
    class cBridgeViewModel : INotifyPropertyChanged
    {
        private string _localServerAddress;
        private cBridgeHttpServer _httpServer;
        private int _port;
             

        public cBridgeViewModel() {
            _port = getFreeTCPPort();
            SetUpHttpServer(_port);
            LocalServerAddress = getLocalIP() + ":" + _port;
        }

        private int getFreeTCPPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        private string getLocalIP() 
        {
            string localIP = "?";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }

            return localIP;
        }

        private void SetUpHttpServer(int port)
        {
            _httpServer = new cBridgeHttpServer(port);
            _httpServer.Start();
        }    

        public string LocalServerAddress {
            get { return _localServerAddress; }
            set {
                _localServerAddress = value;
                NotifyPropertyChanged("LocalServerAddress");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
