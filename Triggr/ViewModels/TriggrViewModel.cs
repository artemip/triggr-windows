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
using Triggr.Events.Reaction;
using Triggr.Networking;
using Triggr.Utilities;

namespace Triggr.ViewModels
{
    public class TriggrViewModel : INotifyPropertyChanged, IDisposable
    {        
        private string _pairingKey;
        private bool _pairingModeEnabled = false;
        private SocketServer _socketServer;

        public static TriggrViewModel Model = new TriggrViewModel();
        public static NotificationViewModel NotificationModel = new NotificationViewModel();

        private TriggrViewModel() {
            var volumeController = new VolumeController();
            var eventHandler = new Events.EventHandler(volumeController);
            var socketMessageHandler = new SocketMessageHandler(eventHandler);

            _socketServer = new SocketServer(socketMessageHandler);
            _socketServer.Start();
        }

        public void Dispose()
        {
            _socketServer.Dispose();
        }

        public static string DeviceID
        {
            get
            {
                string deviceId = Properties.Settings.Default.DeviceID;

                if (deviceId == "")
                {
                    deviceId = Properties.Settings.Default.DeviceID = Guid.NewGuid().ToString();
                    Properties.Settings.Default.Save();
                }

                return deviceId;
            }
        }

        public bool PairingModeEnabled
        {
            get { return _pairingModeEnabled; }

            set
            {
                if (value)
                {
                    var key = new Base62(new Random().Next((int)Math.Pow(62, 5))).ToString(); // Base-62 * 5 characters
                    var padding = new String('0', 5 - key.Length); //Add padding to make 5 characters

                    PairingKey = padding + key;

                    _socketServer.SendPairingKey(PairingKey);
                }
                 
                _pairingModeEnabled = value;
                NotifyPropertyChanged("PairingModeEnabled");
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
