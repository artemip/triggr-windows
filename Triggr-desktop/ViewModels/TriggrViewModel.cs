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

namespace triggr
{
    public enum DeviceStatus { INCOMING_CALL, OUTGOING_CALL, CALL_ENDED, IDLE, NOT_CONNECTED }

    class TriggrViewModel : INotifyPropertyChanged, IDisposable
    {        
        private int _volume = -1;
        private string _pairingKey;
        private bool _pairingModeEnabled = false;
        private bool _serverConnected = false;
        private TriggrHttpServer _httpServer;
        private DeviceStatus _status = DeviceStatus.NOT_CONNECTED;
        private string _callerName;
        private string _callerId;

        public static TriggrViewModel Model = new TriggrViewModel();

        private TriggrViewModel() {            
            _serverConnected = TriggrSocketServer.Start();
            HeartbeatListener.Start();
        }

        public void Dispose()
        {
            if (TriggrSocketServer.Started)
            {
                TriggrSocketServer.Stop();
            }
            VolumeController.Controller.Dispose();
            HeartbeatListener.Stop();
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
            _httpServer = new TriggrHttpServer(port);
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
                    HeartbeatListener.Stop();

                    var key = new Base62(new Random().Next(1000000000)).ToString();
                    var padding = new String('0', 5 - key.Length); //Add padding to make 5 characters

                    PairingKey = padding + key;
                    Status = DeviceStatus.NOT_CONNECTED;

                    TriggrSocketServer.Send("pairing_key:" + PairingKey + "\r\n");
                }
                else
                {
                    HeartbeatListener.Start();
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

        public string CallerName
        {
            get { return _callerName; }
            set
            {
                _callerName = value;
                NotifyPropertyChanged("CallerName");
            }
        }

        public string CallerId
        {
            get { return _callerId; }
            set {
                _callerId = value;
                NotifyPropertyChanged("CallerId");
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
