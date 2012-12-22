using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace cbridge
{
    static class EventHandler
    {
        private static string startCallEvent = "start_call";
        private static string endCallEvent = "end_call";
        private static string pairingSuccessfulEvent = "pairing_successful:(.*)";
        private static string heartbeatEvent = "paired_device_heartbeat";

        public static void handleEvent(string evt)
        {
            if (Regex.IsMatch(evt, startCallEvent)) //Start call
            { 
                cBridgeViewModel.Model.Status = DeviceStatus.CALL_STARTED;
                VolumeController.Controller.OldVolume = VolumeController.Controller.Volume;
                VolumeController.Controller.Volume = 0.05F;
            }
            else if (Regex.IsMatch(evt, endCallEvent)) //End call
            {
                cBridgeViewModel.Model.Status = DeviceStatus.CALL_ENDED;
                VolumeController.Controller.Volume = VolumeController.Controller.OldVolume;
                System.Threading.Thread.Sleep(1000);
                cBridgeViewModel.Model.Status = DeviceStatus.IDLE;
            }
            else if (Regex.IsMatch(evt, pairingSuccessfulEvent))  //Successful pairing
            {
                var phoneId = evt.Split(':')[1];

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
    }
}
