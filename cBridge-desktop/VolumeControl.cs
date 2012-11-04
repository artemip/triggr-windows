using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CoreAudioApi;

namespace cBridge
{
    static class VolumeControl
    {
        private static MMDevice device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia); 
        private static MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();

        public static float getCurrentVolume()
        {
            return device.AudioEndpointVolume.MasterVolumeLevelScalar;
        }
        
        public static void setVolume(float desiredVolume)
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
    }
}
