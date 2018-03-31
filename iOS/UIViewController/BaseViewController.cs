using System;
using MBProgressHUD;
using UIKit;
using CoreGraphics;
using Foundation;

namespace Slink.iOS
{
    public class BaseViewController : AskPermissionViewController
    {
        public BaseViewController() : base() { }
        public BaseViewController(IntPtr handle) : base(handle) { }
        public BaseViewController(string xibName) : base(xibName) { }

        MTMBProgressHUD Hud;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.Clear;

            View.LayoutIfNeeded();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            if (Hud != null && Hud.Window != null)
            {
                Hud.Dispose();
                Hud = null;
            }
        }

        public void ShowHud(String message)
        {
            if (Hud == null)
                Hud = new MTMBProgressHUD(View);

            MTMBProgressHUD.Appearance.TintColor = UIColor.Red;

            Hud.RemoveFromSuperViewOnHide = true;
            Hud.Color = ColorUtils.GetColor(Slink.ColorUtils.ColorType.Theme);
            Hud.LabelText = message;
            Hud.LabelColor = UIColor.White;
            Hud.DimBackground = true;
            View.AddSubview(Hud);
            Hud.Show(true);
        }

        public void HideHud()
        {
            Hud?.Hide(true);
        }

        public void RemoveBackBarButtonTitle()
        {
            if (NavigationController != null && NavigationController.NavigationBar != null && NavigationController.NavigationBar.TopItem != null)
                NavigationController.NavigationBar.TopItem.Title = " ";

            NavigationItem.BackBarButtonItem = new UIBarButtonItem(" ", UIBarButtonItemStyle.Plain, null);
        }
        public void ShowSplashComfirmed(Action compeleted)
        {
            //todo subclass, make it looks nice.

            UIView splash = new UIView(new CGRect(0, 0, 100, 100));
            splash.Center = View.Window.Center;
            splash.BackgroundColor = UIColor.Green;
            splash.Alpha = 0;
            View.AddSubview(splash);

            UIView.Animate(1.0, () =>
            {
                splash.Alpha = 1;
            }, () =>
            {
                UIView.Animate(1.0, () =>
                {
                    splash.Alpha = 0;
                }, () =>
                {
                    splash.RemoveFromSuperview();
                    compeleted?.Invoke();
                });
            });
        }

        public void ShowGenericAlert()
        {
            UIAlertController AlertController = UIAlertController.Create(Strings.Alerts.please_try_again_later, null, UIAlertControllerStyle.Alert);
            AlertController.AddAction(UIAlertAction.Create(Strings.Basic.ok, UIAlertActionStyle.Default, null));
            PresentViewController(AlertController, true, null);
        }
        public void ShowServerConnectivityAlert()
        {
            UIAlertController AlertController = UIAlertController.Create(Strings.Alerts.server_reachability, Strings.Alerts.please_try_again_later, UIAlertControllerStyle.Alert);
            AlertController.AddAction(UIAlertAction.Create(Strings.Basic.ok, UIAlertActionStyle.Default, null));
            PresentViewController(AlertController, true, null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (Hud != null)
                {
                    Hud.Dispose();
                    Hud = null;
                }
            }
        }
    }
}
