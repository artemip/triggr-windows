using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CoreAudioApi;

namespace cbridge
{    
    class VolumeController
    {
        private float oldVolume;
        private MMDeviceEnumerator DevEnum;
        private MMDevice device;

        public static readonly VolumeController Controller = new VolumeController();

        private VolumeController()
        {
            DevEnum = new MMDeviceEnumerator();
            device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            //Hacks to update volume dynamically
            new Thread(delegate()
            {
                while (true)
                {
                    Thread.Sleep(100);
                    cBridgeViewModel.Model.VolumePercentage = (int)(Controller.Volume * 100);
                }
            }).Start();
        }

        public float OldVolume
        {
            get { return oldVolume; }
            set { oldVolume = (value > 100F) ? 100F : value; }
        }

        public float Volume
        {
            get { 
                return device.AudioEndpointVolume.MasterVolumeLevelScalar; 
            }
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
