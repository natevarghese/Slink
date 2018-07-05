using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public class PinterestWebViewController : BaseViewController
    {
        UIWebView WebView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RemoveBackBarButtonTitle();

            //clear cache
            WebUtils.ClearCache();

            //clear cookies
            WebUtils.ClearCookies();

            string AuthorizeUrl = "https://api.pinterest.com/oauth/?";
            AuthorizeUrl += "response_type=code&";
            AuthorizeUrl += "client_id=" + NotSensitive.SlinkKeys.pinterest_client_id + "&";
            AuthorizeUrl += "state=" + "SLINKNILS" + "&";
            AuthorizeUrl += "scope=" + "read_public" + "&";
            AuthorizeUrl += "redirect_uri=" + NotSensitive.SystemUrls.pinterest_redirect_url;
            Console.WriteLine(AuthorizeUrl);

            var webViewDelegate = new PinterestWebViewDelegate();
            webViewDelegate.TokenReceived += async (token) =>
            {
                ShowHud(Strings.Hud.please_wait);

                var result = await PinterestAuthenticator.GetPinterestAccount(token);
                if (result != null)
                {
                    var popToViewController = NavigationController.ViewControllers.Where(c => c.GetType() == typeof(MyOutletsViewController)).First();
                    NavigationController.PopToViewController(popToViewController, true);
                }

                HideHud();
            };


            var request = new NSMutableUrlRequest(NSUrl.FromString(AuthorizeUrl), NSUrlRequestCachePolicy.ReloadIgnoringLocalCacheData, 10);
            request.ShouldHandleCookies = false;

            var offsetY = NavigationController.NavigationBar.Bounds.Height;
            WebView = new UIWebView(new CGRect(0, offsetY, View.Bounds.Width, View.Bounds.Height - offsetY));
            WebView.Delegate = webViewDelegate;
            WebView.LoadRequest(request);
            View.AddSubview(WebView);
            View.AddConstraint(NSLayoutConstraint.Create(WebView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, offsetY));
            View.AddConstraint(NSLayoutConstraint.Create(WebView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, View, NSLayoutAttribute.Right, 1, 0));
            View.AddConstraint(NSLayoutConstraint.Create(WebView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0));
            View.AddConstraint(NSLayoutConstraint.Create(WebView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, View, NSLayoutAttribute.Left, 1, 0));

        }
        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            if (WebView != null)
                WebView.ScrollView.ContentInset = UIEdgeInsets.Zero;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                WebView?.Dispose();
                WebView = null;
            }
        }
    }
    class PinterestWebViewDelegate : UIWebViewDelegate
    {
        public Action<string> TokenReceived;

        public override bool ShouldStartLoad(UIWebView webView, Foundation.NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            var url = request.Url.AbsoluteString;
            if (url.Contains(NotSensitive.SystemUrls.pinterest_redirect_url))
            {
                if (url.Contains(NotSensitive.SystemUrls.instagram_redirect_url + "/?code") || url.Contains("&code"))
                {
                    var separatedByAmpersand = url.Split('&');
                    foreach (string sub in separatedByAmpersand)
                    {
                        if (sub.Contains("code"))
                        {
                            var seperatedByEquals = sub.Split('=');
                            var token = seperatedByEquals.Last();
                            if (!String.IsNullOrEmpty(token))
                            {
                                TokenReceived?.Invoke(token);
                            }
                        }
                    }

                }
            }
            return true;
        }
    }
}
