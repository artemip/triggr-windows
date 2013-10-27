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
        public NotificationWindow(NotificationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
