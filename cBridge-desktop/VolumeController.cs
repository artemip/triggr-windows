using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CoreAudioApi;

namespace cBridge
{    
    class VolumeController
    {
        public static volatile VolumeController controller = new VolumeController();

        private float oldVolume;
        private MMDeviceEnumerator DevEnum;
        private MMDevice device;

        private VolumeController()
        {
            DevEnum = new MMDeviceEnumerator();
            device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
        }

        public float OldVolume
        {
            get { return oldVolume; }
            set { oldVolume = (value > 100F) ? 100F : value; }
        }

        public float getCurrentVolume()
        {
            return device.AudioEndpointVolume.MasterVolumeLevelScalar;
        }
        
        public void setVolume(float desiredVolume)
        {
            int numSteps = 100;
            float initVolume = getCurrentVolume();
            float stepAmount = Math.Abs(initVolume - desiredVolume) / numSteps;

            bool lowerVolume = initVolume > desiredVolume;

            for (int i = 0; i < numSteps; ++i)
            {
                Thread.Sleep(5);
                if (lowerVolume)
                    device.AudioEndpointVolume.MasterVolumeLevelScalar -= stepAmount;
                else
                    device.AudioEndpointVolume.MasterVolumeLevelScalar += stepAmount;
            }
        }

        public void handleEvent(string data)
        {
            if (data == "incoming_call")
            {
                OldVolume = getCurrentVolume();
                setVolume(0.05F);
            }
            else if (data == "call_ended")
            {
                setVolume(OldVolume);
            }
        }

    }
}
