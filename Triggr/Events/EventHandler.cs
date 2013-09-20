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
            if (evt.Handler == "lower_volume")
            {
                _volumeController.LowerVolume();
            }
            else if (evt.Handler == "restore_volume")
            {
                _volumeController.RestoreVolume();
            }
            else if (evt.Handler == "alert_noise")
            {
                //TODO
            }
        }
    }
}
