using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace triggr
{
    /// <summary>
    /// Convert a DeviceStatus to an image's source path
    /// </summary>
    [ValueConversionAttribute(typeof(DeviceStatus), typeof(string))]
    class StatusToStringConverter : IValueConverter
    {
        private string callStartedString = "Incoming Call";
        private string callEndedString = "Call Ended";
        private string notConnectedString = "No Device";
        private string idleString = "Device Idle";

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            DeviceStatus v = (DeviceStatus)value;

            switch (v)
            {
                case DeviceStatus.CALL_STARTED:
                    return callStartedString;
                case DeviceStatus.CALL_ENDED:
                    return callEndedString;
                case DeviceStatus.NOT_CONNECTED:
                    return notConnectedString;
                default:
                    return idleString;
            }            
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            string statusString = (string)value;

            if (statusString == callStartedString) return DeviceStatus.CALL_STARTED;
            else if (statusString == callEndedString) return DeviceStatus.CALL_ENDED;
            else  if (statusString == notConnectedString) return DeviceStatus.NOT_CONNECTED;
            else return DeviceStatus.IDLE;
        }
    }
}
