using System;
using System.Linq;
using System.Reflection;
using Com.Nostra13.Universalimageloader.Core;
using Com.Nostra13.Universalimageloader;
using Android.Graphics;
using Android.Views;
using Com.Nostra13.Universalimageloader.Core.Assist;

namespace Slink.Droid
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
                ImageLoader.Instance.LoadImage(url, new ImageLoadingListener());
            }
        }
    }
    public class ImageLoadingListener : Java.Lang.Object, Com.Nostra13.Universalimageloader.Core.Listener.IImageLoadingListener
    {
        public void OnLoadingCancelled(string p0, View p1)
        {
        }

        public void OnLoadingComplete(string p0, View p1, Bitmap p2)
        {
        }

        public void OnLoadingFailed(string p0, View p1, FailReason p2)
        {
        }

        public void OnLoadingStarted(string p0, View p1)
        {
        }
    }
}
