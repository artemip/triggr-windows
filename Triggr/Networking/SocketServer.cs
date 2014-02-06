using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using Triggr.ViewModels;
using Triggr;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Triggr.Networking
{
    public class SocketServer : IDisposable
    {
        public class SocketServerNotStartedException : Exception
        {
            public SocketServerNotStartedException(string message)
                : base(message)
            { }
        }

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

        private Socket _socket;
        private bool _started;
        private SocketMessageHandler _socketMessageHandler;
        private Timer _heartbeatTimer;

        public SocketServer(SocketMessageHandler socketMessageHandler)
        {
            _socketMessageHandler = socketMessageHandler;
        }

        public void Dispose()
        {
            Stop();
        }

        public bool Start()
        {
            if (_started) return true;

            _socket = SocketUtils.OpenSocketConnection("api.triggrapp.com", 9090);

            if (_socket == null)
            {
                _started = false;
                return false;
            }

            _started = true;

            StateObject state = new StateObject();
            state.workSocket = _socket;

            _socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

            SendHandshake();

            StartHeartbeat();
            
            return true;
        }

        public bool Started
        {
            get { return _started; }
        }

        public void Stop()
        {
            try
            {
                if (_socket != null)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                }

                _started = false;
            }
            finally
            {
                _socket.Close();
            }
        }

        private void ReadCallback(IAsyncResult ar) 
        {
            string content = "";

            var state = (StateObject) ar.AsyncState;
            var socket = state.workSocket;

            if (!socket.Connected) return;

            int bytesRead = 0;

            try
            {
                bytesRead = socket.EndReceive(ar);
            }
            catch (SocketException)
            {
                bytesRead = 0;
            }

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.UTF8.GetString(
                    state.buffer, 0, bytesRead));

                // Check for newline. If it is not there, read more data.
                content = state.sb.ToString();
                if (content.IndexOf("\r\n") > -1)
                {
                    _socketMessageHandler.HandleMessage(content.Replace("\r\n", ""));
                    state.sb.Clear();
                }

                socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            else //Socket has disconnected. Reconnect...
            {
                Timer reconnectTimer = new Timer(x => { }, null, 0, 0);

                _started = false;

                reconnectTimer = new Timer(t =>
                {
                    if (Start()) //Socket has connected
                    {
                        reconnectTimer.Dispose();
                    }
                }, null, 0, (new Random()).Next(5000, 30000)); //Retry every 5 - 30 seconds (avoid server load)
            }
        }

        private void StartHeartbeat()
        {
            _heartbeatTimer = new Timer(t =>
            {
                SendHeartbeat();
            }, null, 0, (new Random()).Next(30000, 60000)); //Retry every 30 - 60 seconds (avoid server load)
        }

        private string CreateJSONRequest(ServerRequest request)
        {
            var memStream = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(ServerRequest));

            serializer.WriteObject(memStream, request);
            memStream.Position = 0;

            return new StreamReader(memStream).ReadToEnd();
        }

        public void SendPairingKey(string pairingKey)
        {
            var request = new ServerRequest("register_pairing_key", pairingKey);
            Send(CreateJSONRequest(request) + "\r\n");
        }

        private void SendHandshake()
        {
            var request = new ServerRequest("register_device", TriggrViewModel.DeviceID);
            Send(CreateJSONRequest(request) + "\r\n");
        }

        private void SendHeartbeat()
        {
            var request = new ServerRequest("heartbeat", "");
            Send(CreateJSONRequest(request) + "\r\n");
        }

        private void Send(string data)
        {
            while (!_started) Start();
            Send(_socket, data);
        }

        private void Send(Socket socket, string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), socket);
        }

        private void SendCallback(IAsyncResult ar)
        {
            var socket = (Socket) ar.AsyncState;
            var bytesSent = socket.EndSend(ar);            
        }
    }
}
