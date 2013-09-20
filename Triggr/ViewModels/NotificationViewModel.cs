using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Triggr.Events;

namespace Triggr.ViewModels
{
    public class NotificationViewModel : INotifyPropertyChanged, IDisposable
    {
        private Event _event;

        public NotificationViewModel()
        {
        }

        public void Dispose()
        {

        }

        public Event Event
        {
            get
            {
                return _event;
            }
            set
            {
                _event = value;
                NotifyPropertyChanged("Event");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
