using System;
using System.Threading.Tasks;
using CoreAnimation;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public static class ApplicationExtensions
    {
        public static void LoadStoryboardRoot(string storyboardName, bool animated)
        {
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;

            if (animated)
            {
                UIView.Transition(appDelegate.Window, 0.5, UIViewAnimationOptions.TransitionCrossDissolve, () =>
                {
                    appDelegate.Window.RootViewController = UIStoryboard.FromName(storyboardName, null).InstantiateInitialViewController();
                }, null);
            }
            else
            {
                appDelegate.Window.RootViewController = UIStoryboard.FromName(storyboardName, null).InstantiateInitialViewController();
            }

        }
        public static void PresentViewControllerOnRoot(UIViewController target, bool animated)
        {
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            var flyingObjectsVC = appDelegate.Window.RootViewController as FlyingObjectsContainterViewController;
            var navigationController = flyingObjectsVC.ContainerNavigationController;
            var masterVC = navigationController.ViewControllers[0] as MasterViewController;
            masterVC.PresentViewController(target, animated, null);
        }
        public static void PushViewController(UIViewController vc, bool animated)
        {
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            var flyingObjectsVC = appDelegate.Window.RootViewController as FlyingObjectsContainterViewController;
            var navigationController = flyingObjectsVC.ContainerNavigationController;
            navigationController.PushViewController(vc, animated);
        }
        async public static void ActivatePage(Type target, bool animated, bool delayedExecution)
        {
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            var flyingObjectsVC = appDelegate.Window.RootViewController as FlyingObjectsContainterViewController;
            var navigationController = flyingObjectsVC.ContainerNavigationController;
            var activatorContainer = navigationController.ViewControllers[0] as MasterViewController;

            var activatorVC = activatorContainer.TargetViewController as ActivatorViewController;
            activatorVC.AutomaticallyAdjustsScrollViewInsets = false;

            var vc = (UIViewController)Activator.CreateInstance(target);
            activatorContainer.Title = vc.Title;

            if (delayedExecution)
                await Task.Delay(TimeSpan.FromSeconds(0.25));

            activatorVC.SetViewController(vc, animated);
        }
        public static void ShowOnboarding(bool animated)
        {
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            var flyingObjectsContainerViewController = new FlyingObjectsContainterViewController();
            flyingObjectsContainerViewController.ShowsAds = false;
            var clearNavigationController = flyingObjectsContainerViewController.ContainerNavigationController;
            clearNavigationController.SetViewControllers(new UIViewController[] { new OnboardingViewController() }, false);

            if (animated)
            {
                UIView.Transition(appDelegate.Window, 0.5, UIViewAnimationOptions.TransitionCrossDissolve, () =>
                {
                    appDelegate.Window.RootViewController = flyingObjectsContainerViewController;
                }, null);
            }
            else
            {
                appDelegate.Window.RootViewController = flyingObjectsContainerViewController;
            }
        }
        public static void EnterApplication(bool animated, bool doubleTake)
        {
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            var flyingObjectsContainerViewController = new FlyingObjectsContainterViewController();
            var clearNavigationController = flyingObjectsContainerViewController.ContainerNavigationController;
            clearNavigationController.SetViewControllers(new UIViewController[] { new MasterViewController() }, false);

            if (animated)
            {
                UIView.Transition(appDelegate.Window, 0.5, UIViewAnimationOptions.TransitionNone, () =>
                {
                    if (appDelegate.Window.RootViewController == null)
                    {
                        appDelegate.Window.RootViewController = flyingObjectsContainerViewController;
                        appDelegate.SetUpPushNotificaions();
                        return;
                    }


                    appDelegate.Window.RootViewController.DismissViewController(false, () =>
                    {
                        appDelegate.Window.RootViewController = flyingObjectsContainerViewController;
                    });

                    appDelegate.SetUpPushNotificaions();


                }, null);
            }
            else
            {
                if (appDelegate.Window.RootViewController == null)
                {
                    appDelegate.Window.RootViewController = flyingObjectsContainerViewController;
                    appDelegate.SetUpPushNotificaions();
                    return;
                }


                appDelegate.Window.RootViewController.DismissViewController(false, () =>
                {
                    appDelegate.Window.RootViewController = flyingObjectsContainerViewController;
                });
            }

            if (doubleTake)
            {
                appDelegate.Window.RootViewController.DismissViewController(false, null);
                appDelegate.Window.RootViewController = flyingObjectsContainerViewController;
            }
        }
        public static void DismissAllViewControllers(bool animated, bool doubleTake)
        {
            EnterApplication(animated, doubleTake);
        }

        public static void OpenApplicationFromOutlet(Outlet item)
        {
            if (item == null) return;
            if (String.IsNullOrEmpty(item.Type)) return;
            if (item.Type.Equals(Outlet.outlet_type_github, StringComparison.InvariantCultureIgnoreCase)) return;

            var url = "";



            if (item.Type.Equals(Outlet.outlet_type_email, StringComparison.InvariantCultureIgnoreCase))
                url = "mailto:" + item.Handle;//todo untested

            else if (item.Type.Equals(Outlet.outlet_type_phone, StringComparison.InvariantCultureIgnoreCase))
                url = "sms:" + item.Handle;

            else if (item.Type.Equals(Outlet.outlet_type_website, StringComparison.InvariantCultureIgnoreCase))
                url = item.Handle;

            else if (item.Type.Equals(Outlet.outlet_type_google, StringComparison.InvariantCultureIgnoreCase))
                url = "gplus://plus.google.com/" + item.Handle;

            else if (item.Type.Equals(Outlet.outlet_type_facebook, StringComparison.InvariantCultureIgnoreCase))
                url = "fb://profile/" + item.Handle;

            else if (item.Type.Equals(Outlet.outlet_type_linkedIn, StringComparison.InvariantCultureIgnoreCase))
                url = "https://www.linkedin.com/in/" + item.Handle;

            else if (item.Type.Equals(Outlet.outlet_type_twitter, StringComparison.InvariantCultureIgnoreCase))
                url = "twitter:///user?screen_name=" + item.Handle;

            else if (item.Type.Equals(Outlet.outlet_type_instagram, StringComparison.InvariantCultureIgnoreCase))
                url = "instagram://user?username=" + item.Handle;

            else if (item.Type.Equals(Outlet.outlet_type_pinterest, StringComparison.InvariantCultureIgnoreCase))
                url = "pinterest://user/" + item.Handle;

            if (UIApplication.SharedApplication.CanOpenUrl(NSUrl.FromString(url)))
                UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(url));
            else
                AppCenterManager.Report("Cannot open url: " + url);
        }
    }
}
