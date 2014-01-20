using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Triggr.ViewModels;

namespace Triggr.Events.Reaction
{
    public class NotificationController
    {
        private bool[] _availableNotificationSpots;
        private readonly TimeSpan _fadeInDuration = TimeSpan.FromSeconds(0.4);
        private readonly TimeSpan _fadeOutDuration = TimeSpan.FromSeconds(0.2);
        private readonly TimeSpan _displayDuration = TimeSpan.FromSeconds(8);
        private readonly int _rightOffset = 8;
        private readonly int _topOffset = 28;
        private readonly int _maxNumNotifications = 5;

        public NotificationController() {
            _availableNotificationSpots = new bool[_maxNumNotifications];
            for (int i = 0; i < _maxNumNotifications; ++i)
            {
                _availableNotificationSpots[i] = true;
            }
        }

        public void notify(Notification notification)
        {
            filterNotification(notification);

            var notificationViewModel = new NotificationViewModel(notification);
            var notificationWindow = new NotificationWindow(notificationViewModel);

            var screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            var screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            var nHeight = notificationWindow.Height;
            var nWidth = notificationWindow.Width;

            var notificationSpot = -1;
            for (int i = 0; i < _maxNumNotifications; ++i) {
                if (_availableNotificationSpots[i]) {
                    notificationSpot = i;
                    break;
                }
            }   
            if(notificationSpot == -1) {
                for (int i = 0; i < _maxNumNotifications; ++i)
                {
                    _availableNotificationSpots[i] = true;
                }
                notificationSpot = 0;
            }

            notificationWindow.Left = screenWidth - nWidth - _rightOffset;
            notificationWindow.Top = _topOffset + notificationSpot * 90;

            notificationWindow.Opacity = 0;
            notificationWindow.Show();
            _availableNotificationSpots[notificationSpot] = false;

            var showAnimation = new DoubleAnimation(0, 1, (Duration)_fadeInDuration);
            showAnimation.Completed += (s1, e1) =>
            {
                var waitAnimation = new DoubleAnimation(1, _displayDuration);
                waitAnimation.Completed += (s2, e2) =>
                {
                    var hideAnimation = new DoubleAnimation(0, (Duration)_fadeOutDuration);
                    hideAnimation.Completed += (s3, e3) =>
                    {
                        notificationWindow.Hide();
                        notificationWindow.Close();
                        _availableNotificationSpots[notificationSpot] = true;
                    };
                    notificationWindow.BeginAnimation(UIElement.OpacityProperty, hideAnimation);
                };
                notificationWindow.BeginAnimation(UIElement.OpacityProperty, waitAnimation);
            };
            notificationWindow.BeginAnimation(UIElement.OpacityProperty, showAnimation);
        }

        private void filterNotification(Notification notification)
        {
            notification.Title = notification.Title.Trim();
            notification.Subtitle = notification.Subtitle.Trim();
            notification.Description = notification.Description.Trim();
        }
    }
}
