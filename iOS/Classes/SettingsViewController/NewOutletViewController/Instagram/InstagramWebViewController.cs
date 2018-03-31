using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public class InstagramWebViewController : BaseViewController
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


            var webViewDelegate = new InstagramWebViewDelegate();
            webViewDelegate.TokenReceived += async (token) =>
            {
                ShowHud(Strings.Hud.please_wait);

                var result = await InstagramAuthenticator.GetInstagramAccount(token);
                if (result != null)
                {
                    var popToViewController = NavigationController.ViewControllers.Where(c => c.GetType() == typeof(MyOutletsViewController)).First();
                    NavigationController.PopToViewController(popToViewController, true);
                }
            };
            webViewDelegate.Reload += () =>
            {
                Load();
            };


            var offsetY = NavigationController.NavigationBar.Bounds.Height;
            WebView = new UIWebView(new CGRect(0, offsetY, View.Bounds.Width, View.Bounds.Height - offsetY));
            WebView.Delegate = webViewDelegate;
            View.AddSubview(WebView);
            View.AddConstraint(NSLayoutConstraint.Create(WebView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, offsetY));
            View.AddConstraint(NSLayoutConstraint.Create(WebView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, View, NSLayoutAttribute.Right, 1, 0));
            View.AddConstraint(NSLayoutConstraint.Create(WebView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0));
            View.AddConstraint(NSLayoutConstraint.Create(WebView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, View, NSLayoutAttribute.Left, 1, 0));

            Load();
        }
        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            if (WebView != null)
                WebView.ScrollView.ContentInset = UIEdgeInsets.Zero;
        }
        void Load()
        {
            string AuthorizeUrl = "https://api.instagram.com/oauth/authorize/?";
            AuthorizeUrl += "response_type=code&";
            AuthorizeUrl += "client_id=" + Strings.SlinkKeys.instagram_client_id + "&";
            AuthorizeUrl += "redirect_uri=" + Strings.SystemUrls.instagram_redirect_url;
            Console.WriteLine(AuthorizeUrl);

            var request = new NSMutableUrlRequest(NSUrl.FromString(AuthorizeUrl), NSUrlRequestCachePolicy.ReloadIgnoringLocalCacheData, 10);
            request.ShouldHandleCookies = true;//do not set to false

            WebView.LoadRequest(request);
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
    class InstagramWebViewDelegate : UIWebViewDelegate
    {
        public Action<string> TokenReceived;
        public Action Reload;

        public override bool ShouldStartLoad(UIWebView webView, Foundation.NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            var url = request.Url.AbsoluteString;
            Console.WriteLine(url);

            if (url.Equals("https://www.instagram.com/accounts/login/", StringComparison.InvariantCultureIgnoreCase))
                Reload?.Invoke();
            if (url.Equals("https://www.instagram.com/", StringComparison.InvariantCultureIgnoreCase))
                Reload?.Invoke();

            if (url.Contains(Strings.SystemUrls.instagram_redirect_url))
            {
                if (url.Contains(Strings.SystemUrls.instagram_redirect_url + "/?code"))
                {
                    var token = url.Substring(url.IndexOf("code=", StringComparison.InvariantCultureIgnoreCase) + 5);
                    if (!String.IsNullOrEmpty(token))
                    {
                        TokenReceived?.Invoke(token);
                    }
                }
            }
            return true;
        }
    }
}
