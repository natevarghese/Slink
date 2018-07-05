using System;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace Slink.Droid
{
    public class GithubWebViewFragment : BaseWebViewFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            Activity.Title = Strings.PageTitles.page_title_github;

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
            if (url.Contains(NotSensitive.SystemUrls.github_redirect_url + "/?code") || url.Contains("&code"))
            {
                var separatedByQestionMark = url.Split('?');
                foreach (string sub in separatedByQestionMark)
                {
                    if (sub.Contains("code"))
                    {
                        var seperatedByEquals = sub.Split('=');
                        var token = seperatedByEquals.Last();
                        if (!String.IsNullOrEmpty(token))
                        {
                            ShowHud(Strings.Hud.please_wait);

                            var result = await GithubAuthenticator.GetGithubAccount(token);
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
            string AuthorizeUrl = "https://github.com/login/oauth/authorize?scope=user:email&client_id=" + NotSensitive.SlinkKeys.github_client_id;
            Console.WriteLine(AuthorizeUrl);
            return AuthorizeUrl;
        }
    }
}



