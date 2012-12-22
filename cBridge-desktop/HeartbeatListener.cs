using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace cbridge
{
    static class HeartbeatListener
    {
        private static Timer _checkTimer = null;
        private static bool _heartbeatFound = false;

        public static void Start()
        {
            _checkTimer = new Timer(obj => {
                _heartbeatFound = false;

                cBridgeSocketServer.Send("heartbeat_check");

                var timeoutTimer = new Timer(x =>
                {
                    if (!_heartbeatFound)
                    {
                        cBridgeViewModel.Model.Status = DeviceStatus.NOT_CONNECTED;
                    }
                }, null, 10000, Timeout.Infinite); //10-second timeout. If no response from server, no heartbeat detected
            }, null, 0, 1800000); //15-minute heartbeat checks
        }

        public static bool HeartbeatFound
        {
            get { return _heartbeatFound; }
            set { 
                _heartbeatFound = value;
                if (value)
                {
                    cBridgeViewModel.Model.Status = DeviceStatus.IDLE;
                }
            }
        }
    }
}
