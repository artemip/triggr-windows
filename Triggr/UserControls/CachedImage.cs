using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Triggr.Networking;

namespace Triggr
{
    public class CachedImage : Image
    {
        private string _imageUri;

        static CachedImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CachedImage), new FrameworkPropertyMetadata(typeof(CachedImage)));
        }

        public static readonly DependencyProperty ImageUriProperty = 
            DependencyProperty.Register(
                "ImageUri",
                typeof(String),
                typeof(CachedImage),
                new PropertyMetadata(default(String), OnImageUriChanged));

        private static void OnImageUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Do something...
            ((CachedImage)d).ImageUri = (String)e.NewValue;
        }

        public string ImageUri
        {
            get
            {
                return _imageUri;
            }
            set
            {
                if (value != _imageUri)
                {
                    Uri uri = null;

                    try
                    {
                        uri = new Uri(FileCache.FromUrl(value));
                    }
                    catch (Exception e)
                    {
                        // TODO: set to default value
                        _imageUri = "";
                        return;
                    }

                    Source = new BitmapImage(uri);
                    _imageUri = value;
                }
            }
        }
    }
}
