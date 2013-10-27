using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Triggr.Events.Reaction;
using Triggr.Networking;
using Triggr.ViewModels;

namespace Triggr.Events
{
    public class EventHandler
    {
        private VolumeController _volumeController;
        private NotificationController _notificationController;

        public EventHandler(VolumeController volumeController, NotificationController notificationController)
        {
            _volumeController = volumeController;
            _notificationController = notificationController;
        }

        public void React(Event evt)
        {
            if (evt.Handlers.Contains("end_pair_mode"))
            {
                runOnUIThread(
                    delegate()
                    {
                        TriggrViewModel.Model.PairingModeEnabled = false;
                    }
                );
            }

            if (evt.Handlers.Contains("notify"))
            {
                runOnUIThread(_notificationController.notify, evt.Notification);
            }
            
            if (evt.Handlers.Contains("lower_volume"))
            {
                _volumeController.LowerVolume();
            }
            
            if (evt.Handlers.Contains("restore_volume"))
            {
                _volumeController.RestoreVolume();
            }
            
            if (evt.Handlers.Contains("alert_noise"))
            {
                //TODO
            }
        }

        public static void runOnUIThread(Action targetMethod)
        {
            var context = new DispatcherSynchronizationContext(Application.Current.Dispatcher);
            context.Post((state) => targetMethod(), null);
        }

        public static void runOnUIThread<T>(Action<T> targetMethod, T param)
        {
            var context = new DispatcherSynchronizationContext(Application.Current.Dispatcher);
            context.Post((state) => targetMethod(param), null);
        }
    }
}
