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
