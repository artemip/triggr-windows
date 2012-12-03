using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace cBridge
{
    static class EventHandler
    {
        static volatile VolumeController controller = new VolumeController();

        public static string handleEvent(string evt)
        {
            if (evt == "verify")
            {
                return evt; //Make this return some confirmation token or some shat
            }
            else
            {
                if (evt == "start_call")
                {
                    controller.OldVolume = controller.Volume;
                    controller.Volume = 0.05F;
                }
                else if (evt == "end_call")
                {
                    controller.Volume = controller.OldVolume;
                }

                return evt;
            }
        }      
    }
}
