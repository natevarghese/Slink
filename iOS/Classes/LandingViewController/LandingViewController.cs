using System;
using UIKit;
using CoreGraphics;
using Foundation;

namespace Slink.iOS
{
    public partial class LandingViewController : BaseViewController
    {
        FlyingObjectsView FlyingObjects;
        NSObject DidEnterBackgroundToken, WillEnterForgroundToken;
        public UIViewController DestinationViewController;

        public LandingViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var service = ServiceLocator.Instance.Resolve<IPersistantStorage>();
            var designType = service.GetDesignType();

            FlyingObjects = new FlyingObjectsView(View.Frame);
            FlyingObjects.StartAnimationLoop(FlyingObjectsView.AnimationDirection.left, designType, 20, 100, 1);
            View.InsertSubview(FlyingObjects, 0);


        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            WillEnterForgroundToken = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, (obj) =>
            {
                var service = ServiceLocator.Instance.Resolve<IPersistantStorage>();
                var designType = service.GetDesignType();
                FlyingObjects.StartAnimationLoop(FlyingObjectsView.AnimationDirection.left, designType, 20, 100, 1);
            });

            DidEnterBackgroundToken = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidEnterBackgroundNotification, (obj) =>
            {
                FlyingObjects.EndAnimation(FlyingObjectsView.AnimationEnding.collapse, 10);
            });
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            if (DidEnterBackgroundToken != null)
            {
                DidEnterBackgroundToken.Dispose();
                DidEnterBackgroundToken = null;
            }
            if (WillEnterForgroundToken != null)
            {
                WillEnterForgroundToken.Dispose();
                WillEnterForgroundToken = null;
            }
        }
        public override void PrepareForSegue(UIStoryboardSegue segue, Foundation.NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            DestinationViewController = segue.DestinationViewController;
        }
        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            if (DestinationViewController != null)
            {
                DestinationViewController.View.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height - UIApplication.SharedApplication.StatusBarFrame.Height);
            }
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (FlyingObjects != null)
                {
                    FlyingObjects.Dispose();
                    FlyingObjects = null;
                }
                if (DestinationViewController != null)
                {
                    DestinationViewController.Dispose();
                    DestinationViewController = null;
                }
                if (DidEnterBackgroundToken != null)
                {
                    DidEnterBackgroundToken.Dispose();
                    DidEnterBackgroundToken = null;
                }
                if (WillEnterForgroundToken != null)
                {
                    WillEnterForgroundToken.Dispose();
                    WillEnterForgroundToken = null;
                }
            }
        }
        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }
    }
}

