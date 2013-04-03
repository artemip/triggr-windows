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
        private string incomingCallString = "Incoming Call";
        private string outgoingCallString = "Outgoing Call";
        private string callEndedString = "Call Ended";
        private string notConnectedString = "No Device";
        private string idleString = "Device Idle";

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            DeviceStatus v = (DeviceStatus)value;

            switch (v)
            {
                case DeviceStatus.INCOMING_CALL:
                    return incomingCallString;
                case DeviceStatus.OUTGOING_CALL:
                    return outgoingCallString;
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

            if (statusString == incomingCallString) return DeviceStatus.INCOMING_CALL;
            else if (statusString == outgoingCallString) return DeviceStatus.OUTGOING_CALL;
            else if (statusString == callEndedString) return DeviceStatus.CALL_ENDED;
            else if (statusString == notConnectedString) return DeviceStatus.NOT_CONNECTED;
            else return DeviceStatus.IDLE;
        }
    }
}
