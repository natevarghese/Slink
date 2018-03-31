using System;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace Slink.Droid
{
    public class PrivacyPolicyFragment : BaseFragment
    {
        public PrivacyPolicyFragment()
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.WebView, container, false);

            var WebView = view.FindViewById<WebView>(Resource.Id.WebView);
            WebView.ClearCache(true);
            WebView.LoadUrl(Strings.SystemUrls.privacy_policy);

            return view;
        }
    }
}
