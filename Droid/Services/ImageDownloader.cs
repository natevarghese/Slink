using System;
using System.Linq;
using System.Reflection;
using Android.Graphics;
using Android.Views;
using FFImageLoading;

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
                ImageService.Instance.LoadUrl(url).Preload();
            }
        }
    }
}
