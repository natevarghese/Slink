using System;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace Slink.Droid
{
    public class LinkedInWebViewFragment : BaseWebViewFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            Activity.Title = Strings.PageTitles.page_title_linkedin;

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
            if (url.Contains(NotSensitive.SystemUrls.linkedin_redirect_url))
            {
                var separatedByQuestionMark = url.Split('?').Last();
                var separatedByAmpersand = separatedByQuestionMark.Split('&');
                foreach (string sub in separatedByAmpersand)
                {
                    if (sub.Contains("code="))
                    {
                        var seperatedByEquals = sub.Split('=');
                        var token = seperatedByEquals.Last();
                        if (!String.IsNullOrEmpty(token))
                        {
                            ShowHud(Strings.Hud.please_wait);

                            var result = await LinkedInAuthenticator.GetLinkedInAccount(token);
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

        public override string GetUrl()
        {
            string AuthorizeUrl = "https://www.linkedin.com/uas/oauth2/authorization?";
            AuthorizeUrl += "response_type=code&";
            AuthorizeUrl += "client_id=" + NotSensitive.SlinkKeys.linkedin_client_id + "&";
            AuthorizeUrl += "redirect_uri=" + NotSensitive.SystemUrls.linkedin_redirect_url + "&";
            AuthorizeUrl += "scope=" + "r_basicprofile" + "&";
            AuthorizeUrl += "state=" + "SLINKNILS";
            Console.WriteLine(AuthorizeUrl);
            return AuthorizeUrl;
        }
    }
}



