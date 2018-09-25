using System;
using System.Threading.Tasks;
using Foundation;
using Plugin.Connectivity;
using UIKit;

namespace Slink.iOS
{
    public class NetworkListenerViewController : SystemEventsViewController
    {
        UILabel Banner;

        public NetworkListenerViewController() : base() { }
        public NetworkListenerViewController(IntPtr handle) : base(handle) { }
        public NetworkListenerViewController(string xibName) : base(xibName) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CrossConnectivity.Current.ConnectivityChanged += (object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e) =>
            {
                if (View.Window != null && IsViewLoaded)
                {
                    if (!e.IsConnected)
                    {
                        ShowBanner();
                    }
                    else
                    {
                        HideBanner();
                    }
                }
            };
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (!CrossConnectivity.Current.IsConnected)
                ShowBanner();

        }

        private void ShowBanner()
        {
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
            for (int i = 0; i < View.Subviews.Length; i++)
            {
                if (View.Subviews[i] == Banner)
                {
                    Banner.RemoveFromSuperview();
                    break;
                }
            }

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
                    Banner.Frame = new CoreGraphics.CGRect(0, 0, UIScreen.MainScreen.Bounds.Size.Width, 30);
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


