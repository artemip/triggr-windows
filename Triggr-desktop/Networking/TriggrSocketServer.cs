using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace triggr
{
    public class SocketServerNotStartedException : Exception
    {
        public SocketServerNotStartedException(string message)
            : base(message)
        { }
    }

    static class TriggrSocketServer
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

        private static string _deviceId = TriggrViewModel.DeviceId();
        private static Socket _socket;
        private static bool _started;

        public static bool Start()
        {
            if (_started) return true;

            _socket = SocketHelper.OpenSocketConnection("api.cbridgeapp.com", 9090);

            if (_socket == null)
            {
                _started = false;
                return false;
            }

            _started = true;

            StateObject state = new StateObject();
            state.workSocket = _socket;

            _socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

            Send("device_id:" + _deviceId + "\r\n");

            return true;
        }

        public static bool Started
        {
            get { return _started; }
        }

        public static void Stop()
        {
            try
            {
                if (_socket != null)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                   //socket.Close();
                }

                _started = false;
            }
            finally
            {
                _socket.Close();
            }
        }

        private static void ReadCallback(IAsyncResult ar) 
        {
            string content = "";

            var state = (StateObject) ar.AsyncState;
            var socket = state.workSocket;

            if (!socket.Connected) return;

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
                    SocketEventHandler.HandleEvent(content.Replace("\r\n", ""));
                    state.sb.Clear();
                }

                socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            else //Socket has disconnected. Reconnect...
            {
                _started = TriggrViewModel.Model.ServerConnected = false; //Inform the Model

                Timer reconnectTimer = new Timer(x => { }, null, 0, 0);

                reconnectTimer = new Timer(t =>
                {
                    if (Start()) //Socket has connected
                    {
                        TriggrViewModel.Model.ServerConnected = true;
                        reconnectTimer.Dispose();
                    }
                }, null, 0, 7000); //Retry every 5 seconds

            }
        }

        public static void Send(string data)
        {
            while (!_started) Start();
            Send(_socket, data);
        }

        private static void Send(Socket socket, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), socket);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            var socket = (Socket) ar.AsyncState;
            var bytesSent = socket.EndSend(ar);            
        }
    }
}
