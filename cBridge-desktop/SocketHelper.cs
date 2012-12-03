using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace cBridge
{
    static class SocketHelper
    {
        public static Socket OpenSocketConnection(string server, int port)
         {
             //string request = "POST /add_device HTTP/1.1\r\nHost: " + server + "\r\nConnection: Close\r\n\r\n";
             
             // Create a socket connection with the specified server and port.
             Socket s = SocketHelper.ConnectSocket(server, port);
 
             if (s == null)
                 return null;
 
             // Send request to the server.
             //SendString(s, request);

             return s;
         }

        public static void SendString(Socket socket, string data)
        {
            Byte[] bytesSent = Encoding.ASCII.GetBytes(data);
            socket.Send(bytesSent, bytesSent.Length, 0);
        }


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
