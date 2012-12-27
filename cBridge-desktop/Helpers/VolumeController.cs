using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CoreAudioApi;

namespace cbridge
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

            //Hacks to update volume dynamically
            _volumeMonitor = new Thread(delegate()
                {
                    while (true)
                    {
                        Thread.Sleep(60);
                        cBridgeViewModel.Model.VolumePercentage = (int)(Controller.Volume * 100);
                    }
                });
            _volumeMonitor.Start();
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
                return _device.AudioEndpointVolume.MasterVolumeLevelScalar; 
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
    }
}
