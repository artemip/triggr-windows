using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Triggr.Networking
{
    [DataContract]
    class ServerRequest
    {
        public ServerRequest(string messageType, string message)
        {
            this.MessageType = messageType;
            this.Message = message;
        }

        [DataMember(Name = "message_type")]
        public string MessageType {get; set;}

        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}
