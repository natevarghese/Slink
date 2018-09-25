using System;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public partial class WebViewViewController : BaseViewController
    {
        public WebViewViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = Transporter.SharedInstance.GetObjectForKey("Title") as string;

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Stop, (sender, e) =>
            {
                ((LandingTabbarController)TabBarController).SetSelectedViewControllerByType(typeof(LoginViewController), false, null);
            });
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var URL = Transporter.SharedInstance.GetObjectForKey("URL") as string;

            if (!String.IsNullOrEmpty(URL))
                WebView.LoadRequest(new NSUrlRequest(NSUrl.FromString(URL)));
        }

    }
}