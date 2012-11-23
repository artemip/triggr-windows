using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

namespace cBridge
{
    class cBridgeHttpServer {

        private int _port;
        private HttpListener listener;

        public cBridgeHttpServer(int port) {
            _port = port;                    
        }

        public void Start()
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            listener = new HttpListener();
            listener.Prefixes.Add("http://*:" + _port + "/");
            listener.Start();
            listener.BeginGetContext(ProcessRequest, listener);
        }

        public void Stop()
        {
            listener.Stop();                
        }

        private void ProcessRequest(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);

            HttpListenerRequest request = context.Request;

            string fullURL = request.RawUrl;
            string evt = fullURL.Substring(fullURL.LastIndexOf('/') + 1);

            string responseString = "OK - " + handleEvent(evt); ;

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);

            context.Response.ContentLength64 = buffer.Length;
            System.IO.Stream output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            listener.BeginGetContext(ProcessRequest, listener);
        }        
        
        private string handleEvent(String evt) 
        {
            if (evt == "verify")
            {
                return evt; //Make this return some confirmation token or some shat
            }
            else
            {
                VolumeController.controller.handleEvent(evt);
                return evt;
            }
        }
    }
}
