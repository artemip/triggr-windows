using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CoreAudioApi;

namespace triggr
{    
    /// <summary>
    /// Interfaces with the VolumeControllerHelper to perform volume-related tasks
    /// </summary>
    class VolumeController : IDisposable
    {
        private float _oldVolume;
        private MMDeviceEnumerator _deviceEnum;
        private MMDevice _device;
        private Thread _volumeMonitor;

        public static readonly VolumeController Controller = new VolumeController();

        /// <summary>
        /// Start the VolumeController
        /// </summary>
        private VolumeController()
        {
            _deviceEnum = new MMDeviceEnumerator();
            _device = _deviceEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);           
        }

        public void Dispose() 
        {
            _volumeMonitor.Abort();
        }

        public float OldVolume
        {
            get { return _oldVolume; }
            set { _oldVolume = (value > 100F) ? 100F : value; }
        }

        public float Volume
        {
            get { 
                float vol = -1;
                while (vol == -1)
                {
                    try
                    {   
                        vol = (_device.AudioEndpointVolume.Mute) ? 0 : _device.AudioEndpointVolume.MasterVolumeLevelScalar;
                    }
                    catch (InvalidCastException ex)
                    {
                    
                    }
                }
                return vol;
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
                        _device.AudioEndpointVolume.MasterVolumeLevelScalar -= stepAmount;
                    else
                        _device.AudioEndpointVolume.MasterVolumeLevelScalar += stepAmount;
                }
            }
        }

        public delegate void VolumeNotificationHandler(AudioVolumeNotificationData data);

        public void SubscribeToVolumeChanges(VolumeNotificationHandler handler)
        {
            _device.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(handler);
        }
    }
}
