using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Triggr.Networking
{
    public class FileCache
    {
        public static string AppCacheDirectory { get; set; }

        static FileCache()
        {
            // default cache directory, can be changed in de app.xaml.
            AppCacheDirectory = String.Format("{0}/Cache/", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        }

        public static string FromUrl(string url)
        {
            //Check to see if the directory in AppData has been created 
            if (!Directory.Exists(AppCacheDirectory))
            {
                //Create it 
                Directory.CreateDirectory(AppCacheDirectory);
            }

            //Cast the string into a Uri so we can access the image name without regex 
            var uri = new Uri(url);
            var localFile = String.Format("{0}{1}", AppCacheDirectory, uri.Segments[uri.Segments.Length - 1]);

            if (!File.Exists(localFile))
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile(url, localFile);
            }

            //The full path of the image on the local computer 
            return localFile;
        }
    }
}
