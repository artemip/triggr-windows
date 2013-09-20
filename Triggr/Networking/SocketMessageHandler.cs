using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Runtime.Serialization.Json;
using Triggr.Networking;
using System.IO;
using Triggr.ViewModels;
using Triggr.Events;

namespace Triggr.Networking
{
    /// <summary>
    /// Handle events that are forwarded from the remote device
    /// </summary>
    public class SocketMessageHandler
    {
        private Object _eventLock = new Object();
        private Events.EventHandler _eventHandler; 

        public SocketMessageHandler(Events.EventHandler eventHandler)
        {
            _eventHandler = eventHandler;
        }

        public void HandleMessage(string eventJSON)
        {
            lock (_eventLock)
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Event));
                Event evt;

                using (Stream stringStream = new MemoryStream(UTF8Encoding.Default.GetBytes(eventJSON ?? "")))
                {
                    evt = (jsonSerializer.ReadObject(stringStream)) as Event;
                }

                TriggrViewModel.NotificationModel.Event = evt;
                _eventHandler.React(evt);
            }
        }
    }
}
