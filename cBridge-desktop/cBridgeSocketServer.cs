using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace cBridge
{
    class cBridgeSocketServer
    {
        private string _deviceId;

        public cBridgeSocketServer(string deviceId)
        {
            _deviceId = deviceId;
        }

        public void Start()
        {
            new Thread(delegate()
            {
                var socket = SocketHelper.OpenSocketConnection("api.cbridgeapp.com", 9090);

                if (socket == null)
                {
                    return;
                }

                //Registration
                SocketHelper.SendString(socket, _deviceId);

                while (socket.Connected)
                {
                    Byte[] bytesReceived = new Byte[256];

                    int bytes = 0;
                    string data = "";

                    do
                    {
                        bytes = socket.Receive(bytesReceived, bytesReceived.Length, 0);
                        data = Encoding.ASCII.GetString(bytesReceived, 0, bytes);

                        EventHandler.handleEvent(data);
                    }
                    while (bytes > 0);
                }
            }).Start();
        }
    }
}
