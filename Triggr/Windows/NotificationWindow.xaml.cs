using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Triggr.ViewModels;

namespace Triggr
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        public NotificationWindow()
        {
            InitializeComponent();
            DataContext = TriggrViewModel.NotificationModel;

            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = screenWidth - windowWidth - 8;
            this.Top = 36;
        }

        private double fadeInDuration = 0.4;
        private double fadeOutDuration = 0.2;

        public void ShowFor(TimeSpan waitDuration)
        {
            this.Opacity = 0;
            this.Show();
            var showAnimation = new DoubleAnimation(0, 1, (Duration)TimeSpan.FromSeconds(fadeInDuration));
            showAnimation.Completed += (s1, e1) =>
            {
                var waitAnimation = new DoubleAnimation(1, waitDuration);
                waitAnimation.Completed += (s2, e2) =>
                {
                    var hideAnimation = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(fadeOutDuration));
                    hideAnimation.Completed += (s3, e3) => this.Hide();
                    this.BeginAnimation(UIElement.OpacityProperty, hideAnimation);
                };
                this.BeginAnimation(UIElement.OpacityProperty, waitAnimation);
            };
            this.BeginAnimation(UIElement.OpacityProperty, showAnimation);
        }
    }
}
