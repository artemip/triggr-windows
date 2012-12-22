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

namespace cbridge
{
    public enum DeviceStatus { CALL_STARTED, CALL_ENDED, IDLE, NOT_CONNECTED }

    class cBridgeViewModel : INotifyPropertyChanged
    {        
        private int _volume = -1;
        private string _pairingKey;
        private bool _pairingModeEnabled = false;
        private bool _serverConnected = false;
        private cBridgeHttpServer _httpServer;
        private DeviceStatus _status = DeviceStatus.NOT_CONNECTED;

        public static volatile cBridgeViewModel Model = new cBridgeViewModel();

        private cBridgeViewModel() {            
            _serverConnected = cBridgeSocketServer.Start();
            HeartbeatListener.Start();
        }

        ~cBridgeViewModel()
        {
            if (cBridgeSocketServer.Started)
            {
                cBridgeSocketServer.Stop();
            }
        }

        public static string DeviceId()
        {
            string deviceId = Properties.Settings.Default.DeviceID;

            if (deviceId == "")
            {
                deviceId = Properties.Settings.Default.DeviceID = Guid.NewGuid().ToString();
                Properties.Settings.Default.Save();
            }

            return deviceId;
        }

        private int getFreeTCPPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
       
        private void SetUpHttpServer(int port)
        {
            _httpServer = new cBridgeHttpServer(port);
            _httpServer.Start();
        }

        public bool ServerConnected { 
            get { return _serverConnected; }
            set {
                _serverConnected = value;
                NotifyPropertyChanged("ServerConnected");
            } 
        }

        public bool PairingModeEnabled
        {
            get { return _pairingModeEnabled; }

            set
            {
                if (value)
                {
                    PairingKey = new Base62(new Random().Next(10000, 1000000000)).ToString();
                    Status = DeviceStatus.NOT_CONNECTED;

                    cBridgeSocketServer.Send("pairing_key:" + PairingKey + "\r\n");
                }

                _pairingModeEnabled = value;
                NotifyPropertyChanged("PairingModeEnabled");
            }
        }

        public int VolumePercentage
        {
            get {
                if (_volume == -1) 
                    _volume = (int)(VolumeController.Controller.Volume * 100);
                
                return _volume; 
            }
            set 
            {
                _volume = value;
                NotifyPropertyChanged("VolumePercentage");
            }
        }

        public DeviceStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }

        public string PairingKey {
            get { return _pairingKey; }
            set {
                _pairingKey = value;
                NotifyPropertyChanged("PairingKey");
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
