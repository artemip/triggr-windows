using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CoreAudioApi;

namespace Triggr
{    
    /// <summary>
    /// Interfaces with the VolumeControllerHelper to perform volume-related tasks
    /// </summary>
    public class VolumeController
    {
        public static float LOW_VOLUME = 0.05F;

        private float _oldVolume;
        private MMDeviceEnumerator _deviceEnum;
        private MMDevice _device;

        public static readonly VolumeController Controller = new VolumeController();

        /// <summary>
        /// Start the VolumeController
        /// </summary>
        private VolumeController()
        {
            _deviceEnum = new MMDeviceEnumerator();
            _device = _deviceEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);           
        }

        public float OldVolume
        {
            get { return _oldVolume; }
            set { _oldVolume = (value > 100F) ? 100F : value; }
        }

        public void LowerVolume()
        {
            LowerVolume(LOW_VOLUME);
        }

        public void LowerVolume(float lowVolume)
        {
            if(Volume > lowVolume) {
                OldVolume = Volume;
                Volume = lowVolume;
            }
        }

        public void RestoreVolume() {
            Volume = OldVolume;
        }

        public void Mute()
        {
            _device.AudioEndpointVolume.Mute = true;
        }

        public void UnMute()
        {
            _device.AudioEndpointVolume.Mute = false;
        }

        public float Volume
        {
            get { 
                float vol = -1;
                while (vol == -1)
                {
                    vol = (_device.AudioEndpointVolume.Mute) ? 0 : _device.AudioEndpointVolume.MasterVolumeLevelScalar;
                }
                return vol;
            }
            set {
                if (value > 1F || value < 0F)
                    throw new ArgumentOutOfRangeException("Volume cannot be over 1 or below 0.");

                int numSteps = 100;
                float initVolume = Volume;
                float stepAmount = Math.Abs(initVolume - value) / numSteps;

                bool lowerVolume = initVolume > value;

                // Iterate for 'numSteps - 1' to avoid over/underflowing (Volume > 1F or <0F)
                for (int i = 0; i < numSteps - 1; ++i)
                {
                    Thread.Sleep(15);
                    if (lowerVolume)
                    {
                        _device.AudioEndpointVolume.MasterVolumeLevelScalar -= stepAmount;
                    }
                    else
                    {
                        _device.AudioEndpointVolume.MasterVolumeLevelScalar += stepAmount;
                    }
                }

                _device.AudioEndpointVolume.MasterVolumeLevelScalar = value;
            }
        }

        public delegate void VolumeNotificationHandler(AudioVolumeNotificationData data);

        public void SubscribeToVolumeChanges(VolumeNotificationHandler handler)
        {
            _device.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(handler);
        }
    }
}
