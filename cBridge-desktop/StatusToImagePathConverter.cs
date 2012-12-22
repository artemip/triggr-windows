using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace cbridge
{
    [ValueConversionAttribute(typeof(DeviceStatus), typeof(string))]
    class StatusToImagePathConverter : IValueConverter
    {
        private string callStartedImagePath = "images/call_started.png";
        private string callEndedImagePath = "images/call_ended.png";
        private string notConnectedImagePath = "images/not_connected.png";
        private string idleImagePath = "images/device_idle.png";

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            DeviceStatus v = (DeviceStatus)value;

            switch (v)
            {
                case DeviceStatus.CALL_STARTED:
                    return callStartedImagePath;
                case DeviceStatus.CALL_ENDED:
                    return callEndedImagePath;
                case DeviceStatus.NOT_CONNECTED:
                    return notConnectedImagePath;
                default:
                    return idleImagePath;
            }            
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            string imagePath = (string)value;

            if (imagePath == callStartedImagePath) return DeviceStatus.CALL_STARTED;
            else if (imagePath == callEndedImagePath) return DeviceStatus.CALL_ENDED;
            else  if (imagePath == notConnectedImagePath) return DeviceStatus.NOT_CONNECTED;
            else return DeviceStatus.IDLE;
        }
    }
}
