using Foundation;
using UIKit;

using Facebook.CoreKit;

using System;
using Google.SignIn;

using Com.OneSignal;
using Com.OneSignal.Abstractions;
using System.Collections.Generic;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Google.MobileAds;

namespace Slink.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {

            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method

            // Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
#endif

            ServiceLocator.Instance.Add<IS3Service, S3Service>();
            ServiceLocator.Instance.Add<IImageDownloader, ImageDownloader>();
            ServiceLocator.Instance.Add<IPersistantStorage, PersistantStorage>();
            ServiceLocator.Instance.Add<IBroadcastNotificaion, BroadCastNotificaion>();

            Profile.EnableUpdatesOnAccessTokenChange(true);
            Settings.AppID = Strings.SlinkKeys.facebook_app_id;
            Settings.DisplayName = Strings.SlinkKeys.facebook_display_name;


            if (!RealmUserServices.DidUserPersist())
            {
                ApplicationExtensions.LoadStoryboardRoot("Landing", false);
            }
            else
            {
                ApplicationExtensions.EnterApplication(false, true);
            }

            PredownloadImages();

            AppCenter.Start("fa06eb43-8be9-426c-97f9-42f3ab13cd3b", typeof(Analytics), typeof(Crashes));
            ServiceRunner.SharedInstance.StartService<AppCenterManager>();

            MobileAds.Configure("ca-app-pub-4252799872870196~2848379026");
            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.

        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            //Facebook
            if (url.AbsoluteString.Contains("fb282730945236494"))
                return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);

            //Google
            if (url.AbsoluteString.Contains("172866788674"))
                return SignIn.SharedInstance.HandleUrl(url, sourceApplication, annotation);


            return true;
        }
        public void PredownloadImages()
        {
            IImageDownloader iImageDownloader = ServiceLocator.Instance.Resolve<IImageDownloader>();
            if (iImageDownloader == null) return;

            iImageDownloader.PredownloadImages();
        }



        #region Push Notificaions
        public void SetUpPushNotificaions()
        {
            OneSignal.Current.StartInit(Strings.SlinkKeys.one_signal_app_id)
                     .InFocusDisplaying(OSInFocusDisplayOption.None)
                     .HandleNotificationReceived(HandleNotificationReceived)
                     .HandleNotificationOpened(HandleNotificationOpened)
                     .EndInit();
            OneSignal.Current.SetLogLevel(LOG_LEVEL.INFO, LOG_LEVEL.FATAL);
            OneSignal.Current.RegisterForPushNotifications();
            OneSignal.Current.IdsAvailable(async (string userID, string pushToken) =>
            {
                Console.WriteLine("UserID:" + userID);
                Console.WriteLine("pushToken:" + pushToken);

                try
                {
                    await WebServices.UserController.UpdateUser(userID);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
            OneSignal.Current.GetTags((Dictionary<string, object> tags) =>
            {
                foreach (var tag in tags)
                    Console.WriteLine(tag.Key + ":" + tag.Value);
            });

            //clear all read
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }
        [Export("oneSignalApplicationDidBecomeActive:")]
        public void OneSignalApplicationDidBecomeActive(UIApplication application)
        {
            Console.WriteLine("OneSignalApplicationDidBecomeActive");
        }

        [Export("oneSignalApplicationWillResignActive:")]
        public void OneSignalApplicationWillResignActive(UIApplication application)
        {
            Console.WriteLine("OneSignalApplicationWillResignActive");
        }
        [Export("oneSignalApplicationDidEnterBackground:")]
        public void OneSignalApplicationDidEnterBackground(UIApplication application)
        {
            Console.WriteLine("oneSignalApplicationDidEnterBackground");
        }

        //this will get called for forground
        private static void HandleNotificationReceived(OSNotification notification)
        {
            HandleNotification(notification, null);
        }
        //this will get called for background, or if clicked in foreground. see InFocusDisplaying during init
        static void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
            HandleNotification(result.notification, result.action.actionID);
        }
        private async static void HandleNotification(OSNotification notification, string actionId)
        {
            var payload = notification.payload;
            var message = payload.body;
            var additionalData = payload.additionalData;

            Console.WriteLine(message);

            if (additionalData != null)
            {
                if (additionalData.ContainsKey(Strings.PushNotificaionKeys.nearby_broadcast))
                {
                    var transactionIdString = additionalData[Strings.PushNotificaionKeys.nearby_broadcast] as NSString;
                    if (transactionIdString == null) return;

                    //increment the discover notificaion count 
                    var persisantStorageService = ServiceLocator.Instance.Resolve<IPersistantStorage>();
                    persisantStorageService.IncrementDiscoverNotificaionCount(1);


                    //give time app to load
                    await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(2));

                    GoToDiscoverPage((actionId == null));
                }
            }
        }
        static void GoToDiscoverPage(bool animated)
        {
            ApplicationExtensions.DismissAllViewControllers(animated, true);
            ApplicationExtensions.ActivatePage(typeof(DiscoverViewController), animated, true);
        }
        #endregion 
    }
}

