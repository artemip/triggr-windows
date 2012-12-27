using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;


namespace cbridge
{

    /// <summary>
    /// Handle events that are received from the remote device
    /// </summary>
    static class SocketEventHandler
    {
        private static string incomingCallEvent = "incoming_call";
        private static string outgoingCallEvent = "outgoing_call";
        private static string endCallEvent = "end_call";
        private static string pairingSuccessfulEvent = "pairing_successful:(.*)";
        private static string heartbeatEvent = "paired_device_heartbeat";

        private static int callCounter = 0;

        /// <summary>
        /// Display a notification that a call is incoming
        /// </summary>
        private static void displayIncomingCallNotification()
        {
            var notification = new Notification(NotificationType.INCOMING_CALL);
            notification.ShowFor(TimeSpan.FromSeconds(4));
        }

        /// <summary>
        /// Display a notification that a call is outgoing
        /// </summary>
        private static void displayOutgoingCallNotification()
        {
            var notification = new Notification(NotificationType.OUTGOING_CALL);
            notification.ShowFor(TimeSpan.FromSeconds(4));
        }

        /// <summary>
        /// Display a notification that a call has ended
        /// </summary>
        private static void displayEndCallNotification()
        {
            var notification = new Notification(NotificationType.CALL_ENDED);
            notification.ShowFor(TimeSpan.FromSeconds(2));
        }


        /// <summary>
        /// Handle an incoming event
        /// </summary>
        /// <param name="evt">The event that has occurred</param>
        public static void HandleEvent(string evt)
        {
            if (Regex.IsMatch(evt, incomingCallEvent)) //Incoming call
            {
                startOnUIThread(displayIncomingCallNotification);

                callCounter++;

                //Only if this is the first call
                if (callCounter == 1)
                {
                    cBridgeViewModel.Model.Status = DeviceStatus.CALL_STARTED;
                    VolumeController.Controller.OldVolume = VolumeController.Controller.Volume;
                    VolumeController.Controller.Volume = 0.05F;
                }
            }
            else if (Regex.IsMatch(evt, outgoingCallEvent)) //Outgoing call
            {
                startOnUIThread(displayOutgoingCallNotification);

                callCounter++;

                //Only if this is the first call
                if (callCounter == 1)
                {
                    cBridgeViewModel.Model.Status = DeviceStatus.CALL_STARTED;
                    VolumeController.Controller.OldVolume = VolumeController.Controller.Volume;
                    VolumeController.Controller.Volume = 0.05F;
                }
            }
            else if (Regex.IsMatch(evt, endCallEvent)) //End call
            {
                startOnUIThread(displayEndCallNotification);

                callCounter--;

                //Last call
                if (callCounter == 0)
                {
                    cBridgeViewModel.Model.Status = DeviceStatus.CALL_ENDED;
                    VolumeController.Controller.Volume = VolumeController.Controller.OldVolume;
                    System.Threading.Thread.Sleep(1000);
                    cBridgeViewModel.Model.Status = DeviceStatus.IDLE;
                }
            }
            else if (Regex.IsMatch(evt, pairingSuccessfulEvent))  //Successful pairing
            {
                var phoneId = evt.Split(':')[1];

                //Save phone's ID
                Properties.Settings.Default.PhoneID = phoneId;
                Properties.Settings.Default.Save();

                cBridgeViewModel.Model.PairingModeEnabled = false;
                cBridgeViewModel.Model.Status = DeviceStatus.IDLE;
            }
            else if (Regex.IsMatch(evt, heartbeatEvent)) //Heartbeat
            {
                HeartbeatListener.HeartbeatFound = true;
            }
        }      

        /// <summary>
        /// Start the specified action on the UI thread
        /// </summary>
        /// <param name="targetMethod"></param>
        private static void startOnUIThread(Action targetMethod) 
        {
            var context = new DispatcherSynchronizationContext(Application.Current.Dispatcher);
            context.Post((state) => targetMethod(), null);
        }
    }
}
