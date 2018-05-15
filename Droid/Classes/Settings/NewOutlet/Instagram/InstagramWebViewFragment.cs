using System;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Webkit;


namespace Slink.Droid
{
    public class InstagramWebViewFragment : BaseWebViewFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            Activity.Title = Strings.PageTitles.page_title_instagram;

            return view;
        }
        public override WebViewClient GetWebViewClient()
        {
            var client = new BaseWebViewClient();
            client.ShouldOverrideUrlLoadingFunc += (IWebResourceRequest arg) =>
            {
                ProcessRequest(arg.Url.ToString());
                return false;
            };
            return client;
        }

        async void ProcessRequest(string url)
        {
            if (url.Equals("https://www.instagram.com/accounts/login/", StringComparison.InvariantCultureIgnoreCase))
                Load();
            if (url.Equals("https://www.instagram.com/", StringComparison.InvariantCultureIgnoreCase))
                Load();

            if (url.Contains(Strings.SystemUrls.instagram_redirect_url))
            {
                if (url.Contains(Strings.SystemUrls.instagram_redirect_url + "/?code"))
                {
                    var token = url.Substring(url.IndexOf("code=", StringComparison.InvariantCultureIgnoreCase) + 5);
                    if (!String.IsNullOrEmpty(token))
                    {
                        ShowHud(Strings.Hud.please_wait);

                        var result = await InstagramAuthenticator.GetInstagramAccount(token);
                        if (result != null)
                        {
                            var activity = Activity as BaseActivity;
                            activity?.PopFragmentOverUntil(typeof(MyOutletsRecyclerViewFragment));
                        }

                        HideHud();
                    }
                }
            }
        }

        void Load()
        {
            var webView = View.FindViewById<WebView>(Resource.Id.WebView);
            webView?.LoadUrl(GetUrl());
        }
        public override string GetUrl()
        {
            string AuthorizeUrl = "https://api.instagram.com/oauth/authorize/?";
            AuthorizeUrl += "response_type=code&";
            AuthorizeUrl += "client_id=" + Strings.SlinkKeys.instagram_client_id + "&";
            AuthorizeUrl += "redirect_uri=" + Strings.SystemUrls.instagram_redirect_url;
            Console.WriteLine(AuthorizeUrl);
            return AuthorizeUrl;
        }
    }
}



