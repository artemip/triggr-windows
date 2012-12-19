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
    class cBridgeViewModel : INotifyPropertyChanged
    {
        public enum DeviceStatus { CALL_STARTED, CALL_ENDED, IDLE, NOT_CONNECTED }

        private int _volume = -1;
        private string _pairingKey;
        private bool _pairingModeEnabled = false;
        private cBridgeHttpServer _httpServer;
        private cBridgeSocketServer _socketServer;
        private int _port;
        private DeviceStatus _status = DeviceStatus.NOT_CONNECTED;

        public static volatile cBridgeViewModel Model = new cBridgeViewModel();

        private cBridgeViewModel() {            
            //_port = getFreeTCPPort();
            //SetUpSocketServer(getDeviceId());       
        }

        ~cBridgeViewModel()
        {
            if (_socketServer != null)
            {
                _socketServer.Stop();
            }
        }

        private string getDeviceId()
        {
            string deviceId = Properties.Settings.Default.DeviceID;

            if (deviceId == "")
            {
                deviceId = Properties.Settings.Default.DeviceID = Guid.NewGuid().ToString();
                Properties.Settings.Default.Save();

                //New device ID. Pairing mode is thereby enabled
                _pairingKey = new Base62(new Random().Next(10000, 1000000000)).ToString();
                _status = DeviceStatus.NOT_CONNECTED;

                _socketServer.Send("pairing_id:" + _pairingKey + "\r\n");
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

        private void SetUpSocketServer(string deviceId)
        {
            _socketServer = new cBridgeSocketServer(deviceId);
            _socketServer.Start();
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
                    
                    _socketServer.Send("pairing_id:" + PairingKey + "\r\n");
                }

                _pairingModeEnabled = value;
                NotifyPropertyChanged("PairingModeTextVisible");
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

        public string StatusImage
        {
            get 
            {                
                switch (_status)
                {
                    case DeviceStatus.CALL_STARTED:
                        return "images/call_started.png";
                    case DeviceStatus.CALL_ENDED:
                        return "images/call_ended.png";
                    case DeviceStatus.NOT_CONNECTED:
                        return "images/not_connected.png";
                    default:
                        return "images/device_idle.png";
                }
                
            }            
        }

        public DeviceStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyPropertyChanged("StatusImage");
            }
        }

        public string PairingModeTextVisible
        {
            get { return (_pairingModeEnabled) ? "Visible" : "Hidden"; }
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
