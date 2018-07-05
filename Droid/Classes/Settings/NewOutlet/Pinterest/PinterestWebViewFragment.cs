using System;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace Slink.Droid
{
    public class PinterestWebViewFragment : BaseWebViewFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            Activity.Title = Strings.PageTitles.page_title_pinterest;

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
                                ShowHud(Strings.Hud.please_wait);

                                var result = await PinterestAuthenticator.GetPinterestAccount(token);
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
            }
        }

        public override string GetUrl()
        {
            string AuthorizeUrl = "https://api.pinterest.com/oauth/?";
            AuthorizeUrl += "response_type=code&";
            AuthorizeUrl += "client_id=" + NotSensitive.SlinkKeys.pinterest_client_id + "&";
            AuthorizeUrl += "state=" + "SLINKNILS" + "&";
            AuthorizeUrl += "scope=" + "read_public" + "&";
            AuthorizeUrl += "redirect_uri=" + NotSensitive.SystemUrls.pinterest_redirect_url;
            Console.WriteLine(AuthorizeUrl);
            return AuthorizeUrl;
        }
    }
}



