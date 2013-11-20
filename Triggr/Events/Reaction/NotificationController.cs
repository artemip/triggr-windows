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
        private int _numNotifications;
        private double _screenWidth;
        private double _screenHeight;
        private readonly TimeSpan _fadeInDuration = TimeSpan.FromSeconds(0.4);
        private readonly TimeSpan _fadeOutDuration = TimeSpan.FromSeconds(0.2);
        private readonly TimeSpan _displayDuration = TimeSpan.FromSeconds(8);

        public NotificationController() {
            _numNotifications = 0;
            _screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            _screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
        }

        public void notify(Notification notification)
        {
            filterNotification(notification);

            var notificationViewModel = new NotificationViewModel(notification);
            var notificationWindow = new NotificationWindow(notificationViewModel);

            var nHeight = notificationWindow.Height;
            var nWidth = notificationWindow.Width;

            notificationWindow.Left = _screenWidth - nWidth - 8;
            notificationWindow.Top = 38 + _numNotifications * 40;

            notificationWindow.Opacity = 0;
            notificationWindow.Show();
            _numNotifications++;

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
                        _numNotifications--;
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
