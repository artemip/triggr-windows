using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Triggr.Networking;
using Triggr.Events;
using Moq;

namespace TriggrTests
{
    [TestClass]
    public class SocketMessageHandlerTest
    {
        private string CreateTestJson(string[] handler)
        {
            return 
                @"{" +
                    "\"sender_id\":\"test_sender_id\"," +
                    "\"type\":\"test_type\"," +
                    "\"notification\":{" +
                        "\"icon_uri\":\"test_icon_uri\"," +
                        "\"title\":\"test_title\"," +
                        "\"subtitle\":\"test_subtitle\"," +
                        "\"description\":\"test_description\"" +
                    "}," +
                    "\"handlers\":{\"" + String.Join("\",\"", handler) + "\"}" +
                "}";
        }

        [TestMethod]
        public void TestLowerVolumeEvent()
        {
            var volumeControllerMock = new Mock<Triggr.Events.Reaction.VolumeController>();
            var notificationControllerMock = new Mock<Triggr.Events.Reaction.NotificationController>();
            var eventHandler = new Triggr.Events.EventHandler(volumeControllerMock.Object, notificationControllerMock.Object);
            volumeControllerMock.Setup(v => v.LowerVolume()).Verifiable();

            var eventJSON = CreateTestJson(new string[] { "lower_volume" });

            var socketMessageHandler = new SocketMessageHandler(eventHandler);
            socketMessageHandler.HandleMessage(eventJSON);

            volumeControllerMock.Verify(v => v.LowerVolume(), Times.Once);
        }

        [TestMethod]
        public void TestRestoreVolumeEvent()
        {
            var volumeControllerMock = new Mock<Triggr.Events.Reaction.VolumeController>();
            var notificationControllerMock = new Mock<Triggr.Events.Reaction.NotificationController>();
            var eventHandler = new Triggr.Events.EventHandler(volumeControllerMock.Object, notificationControllerMock.Object);
            volumeControllerMock.Setup(v => v.RestoreVolume()).Verifiable();

            var eventJSON = CreateTestJson(new string[] { "restore_volume" });

            var socketMessageHandler = new SocketMessageHandler(eventHandler);
            socketMessageHandler.HandleMessage(eventJSON);

            volumeControllerMock.Verify(v => v.RestoreVolume(), Times.Once);
        }

        [TestMethod]
        public void TestLowerVolumeAndNotifyEvent()
        {
            var volumeControllerMock = new Mock<Triggr.Events.Reaction.VolumeController>();
            var notificationControllerMock = new Mock<Triggr.Events.Reaction.NotificationController>();
            var eventHandler = new Triggr.Events.EventHandler(volumeControllerMock.Object, notificationControllerMock.Object);
            volumeControllerMock.Setup(v => v.RestoreVolume()).Verifiable();

            var eventJSON = CreateTestJson(new string[] { "restore_volume", "notify" });

            var socketMessageHandler = new SocketMessageHandler(eventHandler);
            socketMessageHandler.HandleMessage(eventJSON);

            volumeControllerMock.Verify(v => v.RestoreVolume(), Times.Once);
        }
    }
}
