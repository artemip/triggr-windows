using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace cbridge
{
    class cBridgeSocketServer
    {
        // State object for reading client data asynchronously
        private class StateObject
        {
            // Client  socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 1024;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }

        private string _deviceId;
        private Socket _socket;

        public cBridgeSocketServer(string deviceId)
        {
            _deviceId = deviceId;
        }

        public void Start()
        {
            _socket = SocketHelper.OpenSocketConnection("api.cbridgeapp.com", 9090);


            if (_socket == null)
            {
                return;
            }

            StateObject state = new StateObject();
            state.workSocket = _socket;

            _socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

            Send("device_id:" + _deviceId + "\r\n");
        }

        public void Stop()
        {
            if (_socket != null)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
        }

        private void ReadCallback(IAsyncResult ar) 
        {
            string content = "";

            var state = (StateObject) ar.AsyncState;
            var socket = state.workSocket;

            int bytesRead = socket.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for newline. If it is not there, read more data.
                content = state.sb.ToString();
                if (content.IndexOf("\r\n") > -1)
                {
                    EventHandler.handleEvent(content.Replace("\r\n", ""));
                    state.sb.Clear();
                }
                
                socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
        }

        public void Send(string data)
        {
            Send(_socket, data);
        }

        private void Send(Socket socket, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), socket);
        }

        private void SendCallback(IAsyncResult ar)
        {
            var socket = (Socket) ar.AsyncState;
            var bytesSent = socket.EndSend(ar);            
        }

    }
}
