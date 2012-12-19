using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace cbridge
{
    static class EventHandler
    {        
        public static string handleEvent(string evt)
        {
            switch(evt) {
                case "start_call":
                    cBridgeViewModel.Model.Status = cBridgeViewModel.DeviceStatus.CALL_STARTED;
                    VolumeController.Controller.OldVolume = VolumeController.Controller.Volume;
                    VolumeController.Controller.Volume = 0.05F;
                    break;
                case "end_call":
                    cBridgeViewModel.Model.Status = cBridgeViewModel.DeviceStatus.CALL_ENDED;
                    VolumeController.Controller.Volume = VolumeController.Controller.OldVolume;
                    cBridgeViewModel.Model.Status = cBridgeViewModel.DeviceStatus.IDLE;
                    break;
                case "pairing_successful":
                    cBridgeViewModel.Model.PairingModeEnabled = false;
                    cBridgeViewModel.Model.Status = cBridgeViewModel.DeviceStatus.IDLE;
                    break;
                default:
                    break;

            }

            return evt;
        }      
    }
}
