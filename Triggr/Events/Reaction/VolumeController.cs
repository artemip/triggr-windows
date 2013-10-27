using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CoreAudioApi;

namespace Triggr.Events.Reaction
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


        private void _setEndpointVolume()
        {
            _deviceEnum = new MMDeviceEnumerator();
            _device = _deviceEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
        }

        /// <summary>
        /// Safe method to access the system volume
        /// </summary>
        private AudioEndpointVolume _getEndpointVolume()
        {
            if (_device == null) _setEndpointVolume();

            try
            {
                return _device.AudioEndpointVolume;
            }
            catch (InvalidCastException ex) // Due to threading issues
            {
                _setEndpointVolume();
                return _device.AudioEndpointVolume;
            }
        }

        public float OldVolume
        {
            get { return _oldVolume; }
            set { _oldVolume = (value > 100F) ? 100F : value; }
        }

        public virtual void LowerVolume()
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

        public virtual void RestoreVolume() {
            Volume = OldVolume;
        }

        public void Mute()
        {
            _getEndpointVolume().Mute = true;
        }

        public void UnMute()
        {
            _getEndpointVolume().Mute = false;
        }

        public float Volume
        {
            get { 
                float vol = -1;
                while (vol == -1)
                {
                    vol = (_getEndpointVolume().Mute) ? 0 : _getEndpointVolume().MasterVolumeLevelScalar;
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
                        _getEndpointVolume().MasterVolumeLevelScalar -= stepAmount;
                    }
                    else
                    {
                        _getEndpointVolume().MasterVolumeLevelScalar += stepAmount;
                    }
                }

                _getEndpointVolume().MasterVolumeLevelScalar = value;
            }
        }

        public delegate void VolumeNotificationHandler(AudioVolumeNotificationData data);

        public void SubscribeToVolumeChanges(VolumeNotificationHandler handler)
        {
            _getEndpointVolume().OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(handler);
        }
    }
}
