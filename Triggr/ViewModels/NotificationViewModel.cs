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
        private Notification _notification;

        public NotificationViewModel(Notification notification) {
            Notification = notification;   
        }

        public void Dispose() { }

        public Notification Notification
        {
            get
            {
                return _notification;
            }
            set
            {
                _notification = value;
                NotifyPropertyChanged("Notification");
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
