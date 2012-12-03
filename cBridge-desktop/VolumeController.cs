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
        private float oldVolume;
        private MMDeviceEnumerator DevEnum;
        private MMDevice device;

        public VolumeController()
        {
            DevEnum = new MMDeviceEnumerator();
            device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
        }

        public float OldVolume
        {
            get { return oldVolume; }
            set { oldVolume = (value > 100F) ? 100F : value; }
        }

        public float Volume
        {
            get { return device.AudioEndpointVolume.MasterVolumeLevelScalar; }
            set {
                int numSteps = 100;
                float initVolume = Volume;
                float stepAmount = Math.Abs(initVolume - value) / numSteps;

                bool lowerVolume = initVolume > value;

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
}
