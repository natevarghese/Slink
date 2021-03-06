﻿using System;
using System.Linq;
using System.Reflection;
using Foundation;
using SDWebImage;

namespace Slink.iOS
{
    public class ImageDownloader : IImageDownloader
    {
        public void PredownloadImages()
        {
            var outletTypes = new Outlet().GetType().GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType == typeof(string)).ToList();
            foreach (var p in outletTypes.Where(c => !c.Name.Contains("version")))
            {
                var outlet = new Outlet();
                outlet.Type = p.GetValue(null) as string;

                var url = outlet.RemoteURL;
                SDWebImageManager.SharedManager.ImageDownloader.DownloadImage(new NSUrl(url), SDWebImageDownloaderOptions.IgnoreCachedResponse, null, (image, data, error, finished) =>
                 {
                     if (error != null)
                         Console.WriteLine(error.Description);
                 });
            }
        }
    }
}
