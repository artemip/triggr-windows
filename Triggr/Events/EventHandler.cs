using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Triggr.Events.Reaction;
using Triggr.Networking;

namespace Triggr.Events
{
    public class EventHandler
    {
        private VolumeController _volumeController;

        public EventHandler(VolumeController volumeController)
        {
            _volumeController = volumeController;
        }

        public void React(Event evt)
        {
            if (evt.Handlers.Contains("notify"))
            {
                //Display notification
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
    }
}
