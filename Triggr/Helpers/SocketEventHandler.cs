﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;


namespace Triggr
{

    /// <summary>
    /// Handle events that are received from the remote device
    /// </summary>
    static class SocketEventHandler
    {
        private static string incomingCallEvent = "incoming_call:(.*)";
        private static string outgoingCallEvent = "outgoing_call:(.*)";
        private static string endCallEvent = "end_call";
        private static string pairingSuccessfulEvent = "pairing_successful:(.*)";
        private static string heartbeatEvent = "paired_device_heartbeat";
        private static string disconnectEvent = "paired_device_disconnected";

        private static int callCounter = 0;
        private static Object eventLock = new Object();

        /// <summary>
        /// Handle an incoming event
        /// </summary>
        /// <param name="evt">The event that has occurred</param>
        public static void HandleEvent(string evt)
        {
            lock (eventLock)
            {
                if (Regex.IsMatch(evt, incomingCallEvent)) //Incoming call
                {
                    var data = evt.Split(':')[1];
                    var callerId = data.Split(',')[0];
                    var callerName = data.Split(',')[1];

                    callCounter++;

                    TriggrViewModel.Model.Status = DeviceStatus.INCOMING_CALL;
                    TriggrViewModel.Model.CallerId = callerId;
                    TriggrViewModel.Model.CallerName = callerName;

                    startOnUIThread(displayNotification);

                    //Only if this is the first call
                    if (callCounter == 1)
                    {
                        VolumeController.Controller.LowerVolume();
                    }
                }
                else if (Regex.IsMatch(evt, outgoingCallEvent)) //Outgoing call
                {
                    var data = evt.Split(':')[1];
                    var callerId = data.Split(',')[0];
                    var callerName = data.Split(',')[1];

                    callCounter++;

                    TriggrViewModel.Model.Status = DeviceStatus.OUTGOING_CALL;
                    TriggrViewModel.Model.CallerName = callerName;
                    TriggrViewModel.Model.CallerId = callerId;

                    startOnUIThread(displayNotification);

                    //Only if this is the first call
                    if (callCounter == 1)
                    {
                        VolumeController.Controller.LowerVolume();
                    }
                }
                else if (Regex.IsMatch(evt, endCallEvent)) //End call
                {
                    startOnUIThread(displayNotification);

                    if(callCounter > 0) callCounter--;

                    //Last call
                    if (callCounter == 0)
                    {
                        TriggrViewModel.Model.Status = DeviceStatus.CALL_ENDED;
                        VolumeController.Controller.RestoreVolume();
                        System.Threading.Thread.Sleep(2000);
                        TriggrViewModel.Model.Status = DeviceStatus.IDLE;
                    }
                }
                else if (Regex.IsMatch(evt, pairingSuccessfulEvent))  //Successful pairing
                {
                    var phoneId = evt.Split(':')[1];

                    //Save phone's ID
                    Properties.Settings.Default.PhoneID = phoneId;
                    Properties.Settings.Default.Save();

                    TriggrViewModel.Model.PairingModeEnabled = false;
                    TriggrViewModel.Model.Status = DeviceStatus.IDLE;

                    TriggrViewModel.Model.CallerName = "Connected";
                    TriggrViewModel.Model.CallerId = "";

                    startOnUIThread(displayNotification);
                }
                else if (Regex.IsMatch(evt, heartbeatEvent)) //Heartbeat
                {
                    HeartbeatListener.HeartbeatFound = true;
                }
                else if (Regex.IsMatch(evt, disconnectEvent))
                {
                    HeartbeatListener.HeartbeatFound = false;
                    Properties.Settings.Default.PhoneID = "";
                    Properties.Settings.Default.Save();

                    TriggrViewModel.Model.Status = DeviceStatus.NOT_CONNECTED;

                    TriggrViewModel.Model.CallerName = "Disconnected";
                    TriggrViewModel.Model.CallerId = "";

                    startOnUIThread(displayNotification);
                }
            }
        }

        /// <summary>
        /// Display a notification
        /// </summary>
        public static void displayNotification()
        {
            var notification = new NotificationWindow();
            notification.ShowFor(TimeSpan.FromSeconds(8));
        }

        /// <summary>
        /// Start the specified action on the UI thread
        /// </summary>
        /// <param name="targetMethod"></param>
        public static void startOnUIThread(Action targetMethod)
        {
            var context = new DispatcherSynchronizationContext(Application.Current.Dispatcher);
            context.Post((state) => targetMethod(), null);
        }

        private static void startOnUIThread<T>(Action<T> targetMethod, T param)
        {
            var context = new DispatcherSynchronizationContext(Application.Current.Dispatcher);
            context.Post((state) => targetMethod(param), null);
        }
    }
}