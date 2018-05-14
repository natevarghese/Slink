using System;

using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.CurrentActivity;

namespace Slink.Droid
{
    //You can specify additional application information in this attribute
    [Application(HardwareAccelerated = true, LargeHeap = true)]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer) { }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);

            ServiceLocator.Instance.Add<IS3Service, S3Service>();
            ServiceLocator.Instance.Add<IImageDownloader, ImageDownloader>();
            ServiceLocator.Instance.Add<IPersistantStorage, PersistantStorage>();
            ServiceLocator.Instance.Add<IBroadcastNotificaion, BroadCastNotificaion>();

            PredownloadImages();

            AppCenter.Start("c475bcf9-7882-44a9-af80-1d234aa6d669", typeof(Analytics), typeof(Crashes));
            ServiceRunner.SharedInstance.StartService<AppCenterManager>();

            MobileAds.Initialize(ApplicationContext, "ca-app-pub-4252799872870196~8404684386");
        }

        public override void OnTrimMemory([GeneratedEnum] TrimMemory level)
        {
            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            base.OnTrimMemory(level);
        }
        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
        }


        public void PredownloadImages()
        {
            IImageDownloader iImageDownloader = ServiceLocator.Instance.Resolve<IImageDownloader>();
            if (iImageDownloader == null) return;

            iImageDownloader.PredownloadImages();
        }
    }
}