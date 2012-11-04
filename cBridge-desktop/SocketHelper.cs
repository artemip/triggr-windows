using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace cBridge_desktop
{
    /*
     * -            var socket = OpenSocketConnection("ec2-75-101-183-71.compute-1.amazonaws.com", 9090);
-
-            if(socket == null)
-            {
-                addEventToList("Connection Failed.", eventsListBox);
-                return;
-            }
-
-            addEventToList("Connected.", eventsListBox);
-
-            while(socket.Connected)
-            {
-                Byte[] bytesReceived = new Byte[256];
-
-                int bytes = 0;
-                string data = "";
-
-                do
-                {
-                    bytes = socket.Receive(bytesReceived, bytesReceived.Length, 0);
-                    data = Encoding.ASCII.GetString(bytesReceived, 0, bytes);
-                   
-                    handleCallEvent(data);
-                }
-                while (bytes > 0);
-            }

     */
    static class SocketHelper
    {
        public static Socket OpenSocketConnection(string server, int port)
-        {
-            string request = "GET / HTTP/1.1\r\nHost: " + server +
-                "\r\nConnection: Close\r\n\r\n";
-            Byte[] bytesSent = Encoding.ASCII.GetBytes(request);
-            
-            // Create a socket connection with the specified server and port.
-            Socket s = SocketHelper.ConnectSocket(server, port);
-
-            if (s == null)
-                return null;
-
-            // Send request to the server.
-            s.Send(bytesSent, bytesSent.Length, 0);
-
-            return s;
-        }


        public static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid 
            // an exception that occurs when the host IP Address is not compatible with the address family 
            // (typical in the IPv6 case). 
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
            }
            return s;
        }
    }
}
