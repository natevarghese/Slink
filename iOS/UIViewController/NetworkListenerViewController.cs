using System;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Plugin.Connectivity;
using UIKit;

namespace Slink.iOS
{
    public class NetworkListenerViewController : SystemEventsViewController
    {
        UILabel Banner;
        protected bool NetworkListenerEnabled = true;

        public NetworkListenerViewController() : base() { }
        public NetworkListenerViewController(IntPtr handle) : base(handle) { }
        public NetworkListenerViewController(string xibName) : base(xibName) { }

        public bool IsConnected
        {
            get { return CrossConnectivity.Current.IsConnected; }
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        }

        void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            if (View.Window == null || !IsViewLoaded) return;

            if (!e.IsConnected)
                ShowBanner();
            else
                HideBanner();
        }


        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (!IsConnected)
                ShowBanner();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            CrossConnectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
        }

        public void ShowBanner()
        {
            if (!NetworkListenerEnabled) return;

            if (Banner == null)
            {
                Banner = new UILabel(new CoreGraphics.CGRect(0, -30, UIScreen.MainScreen.Bounds.Size.Width, 30));
                Banner.BackgroundColor = UIColor.Red;
                Banner.Text = "No Internet Found";
                Banner.TextColor = UIColor.White;
                Banner.Font = UIFont.SystemFontOfSize(12);
                Banner.TextAlignment = UITextAlignment.Center;
            }

            //is currently displayed
            View.Subviews.Where(c => c == Banner).FirstOrDefault()?.RemoveFromSuperview();

            View.AddSubview(Banner);
            View.BringSubviewToFront(Banner);

            UIView.Animate(.5, delegate
            {
                if (NavigationController != null && NavigationController.NavigationBar.Translucent)
                {
                    Banner.Frame = new CoreGraphics.CGRect(0, NavigationController.NavigationBar.Frame.Height, UIScreen.MainScreen.Bounds.Size.Width, 30);
                }
                else
                {
                    Banner.Frame = new CoreGraphics.CGRect(0, UIApplication.SharedApplication.StatusBarFrame.Height, UIScreen.MainScreen.Bounds.Size.Width, 30);
                }
            }, null);
        }

        private void HideBanner()
        {
            if (Banner == null) return;

            UIView.Animate(.5, delegate
            {
                Banner.Frame = new CoreGraphics.CGRect(0, -30, UIScreen.MainScreen.Bounds.Size.Width, 30);
            }, delegate
            {
                if (Banner != null)
                {
                    Banner.RemoveFromSuperview();
                    Banner.Dispose();
                    Banner = null;
                }
            });
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Banner?.Dispose();
                Banner = null;
            }
        }
    }
}


