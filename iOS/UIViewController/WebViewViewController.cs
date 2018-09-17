using System;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public partial class WebViewViewController : BaseViewController
    {
        UIBarButtonItem StopBarButtonItem;

        public WebViewViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            NetworkListenerEnabled = false;

            base.ViewDidLoad();

            Title = Transporter.SharedInstance.GetObjectForKey("Title") as string;

            ShowHud("Loading");
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            StopBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Stop, HandleEventHandler);
            NavigationItem.LeftBarButtonItem = StopBarButtonItem;

            var URL = Transporter.SharedInstance.GetObjectForKey("URL") as string;

            if (!String.IsNullOrEmpty(URL))
                WebView.LoadRequest(new NSUrlRequest(NSUrl.FromString(URL)));

            WebView.LoadFinished += WebView_LoadFinished;
            WebView.LoadError += WebView_LoadFinished;
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            StopBarButtonItem.Clicked -= HandleEventHandler;
            StopBarButtonItem?.Dispose();
            StopBarButtonItem = null;

            WebView.LoadFinished -= WebView_LoadFinished;
            WebView.LoadError -= WebView_LoadFinished;
        }

        void HandleEventHandler(object sender, EventArgs e)
        {
            ((LandingTabbarController)TabBarController).SetSelectedViewControllerByType(typeof(LoginViewController), false, null);

        }


        void WebView_LoadFinished(object sender, EventArgs e)
        {
            HideHud();
        }

#if DEBUG
        ~WebViewViewController()
        {
            Console.WriteLine("Finalizer Called WebViewViewController");
        }
#endif
    }
}