using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace triggr
{
    static class HeartbeatListener
    {
        private static Timer _checkTimer = null;
        private static bool _heartbeatFound = false;
        private static bool _started;

        /// <summary>
        /// Start listening for heartbeats evert 5 minutes, with a 10-second timeout if no heartbeat is detected
        /// </summary>
        public static void Start()
        {
            _started = true;

            _checkTimer = new Timer(obj => {
                _heartbeatFound = false;

                var pairedDeviceId = Properties.Settings.Default.PhoneID;

                if (pairedDeviceId != "")
                {
                    TriggrSocketServer.Send("heartbeat_check:" + pairedDeviceId + "\r\n");

                    var timeoutTimer = new Timer(x =>
                    {
                        if (!_heartbeatFound)
                        {
                            TriggrViewModel.Model.Status = DeviceStatus.NOT_CONNECTED;
                        }
                        else
                        {
                            TriggrViewModel.Model.Status = DeviceStatus.IDLE;
                        }
                    }, null, 10000, Timeout.Infinite); //10-second timeout. If no response from server, no heartbeat detected
                }
            }, null, 0, 300000); //5-minute heartbeat checks
        }

        public static void Stop()
        {
            _checkTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _heartbeatFound = false;
            _started = false;
        }

        public static bool Started
        {
            get { return _started; }
            set { _started = value; }
        }

        /// <summary>
        /// Gets or sets the status indicating whether a heartbeat has been detected
        /// </summary>
        public static bool HeartbeatFound
        {
            get { return _heartbeatFound; }
            set { 
                _heartbeatFound = value;
                if (value && _started)
                {
                    TriggrViewModel.Model.Status = DeviceStatus.IDLE;
                }
            }
        }
    }
}
