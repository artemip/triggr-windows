using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Triggr.Events
{
    [DataContract]
    public class Event
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "notification")]
        public Notification Notification { get; set; }

        [DataMember(Name = "handler")]
        public string Handler { get; set; }
    }

    [DataContract]
    public class Notification
    {
        [DataMember(Name = "icon_uri")]
        public string IconURI { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "subtitle")]
        public string Subtitle { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}
