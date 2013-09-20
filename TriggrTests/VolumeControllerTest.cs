using System;
using Triggr;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Triggr.Events.Reaction;

namespace TriggrTests
{
    [TestClass]
    public class VolumeControllerTest
    {
        VolumeController volControl = new VolumeController();
        float volume;

        [TestInitialize]
        public void Init()
        {
            volume = volControl.Volume;
        }

        private void VolumeTest(float newVolume)
        {
            volControl.Volume = newVolume;
            Assert.AreEqual(newVolume, volControl.Volume, 0.001F);
        }

        [TestMethod]
        public void SetVolumeTest()
        {
            VolumeTest(0.3F);
        }

        [TestMethod]
        public void SetFullVolumeTest()
        {
            VolumeTest(1F);
        }

        [TestMethod]
        public void SetNoVolumeTest()
        {
            VolumeTest(0F);
        }

        [TestMethod]
        public void LowerVolumeTest()
        {
            volControl.LowerVolume();

            Assert.AreEqual(VolumeController.LOW_VOLUME, volControl.Volume, 0.001F);
        }

        [TestMethod]
        public void MuteTest()
        {
            float oldVol = volControl.Volume;

            volControl.Mute();
            Assert.AreEqual(0, volControl.Volume);

            volControl.UnMute();
            Assert.AreEqual(oldVol, volControl.Volume);
        }

        [TestMethod]
        public void VolumeSubscriptionTest()
        {
            CoreAudioApi.AudioVolumeNotificationData lastData = null;

            volControl.SubscribeToVolumeChanges(delegate(CoreAudioApi.AudioVolumeNotificationData data) {
                lastData = data;    
            });

            volControl.Volume = 0F;

            volControl.Volume = 0.6F;
            Thread.Sleep(20);
            Assert.AreEqual(0.6F, lastData.MasterVolume, 0.001F);

            volControl.Volume = 0.2F;
            Thread.Sleep(20);
            Assert.AreEqual(0.2F, lastData.MasterVolume, 0.001F);

            volControl.Mute();
            Thread.Sleep(20);
            Assert.IsTrue(lastData.Muted);

            volControl.UnMute();
            Thread.Sleep(20);
            Assert.IsFalse(lastData.Muted);
        }

        [TestCleanup]
        public void Cleanup()
        {
            volControl.UnMute();
            volControl.Volume = volume;
        }
    }
}
