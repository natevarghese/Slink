using System;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace Slink.Droid
{
    public abstract class BaseWebViewFragment : BaseFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.WebView, container, false);

            CookieManager.Instance.RemoveAllCookies(null);
            CookieManager.Instance.Flush();

            var WebView = view.FindViewById<WebView>(Resource.Id.WebView);
            WebView.ClearHistory();
            WebView.ClearCache(true);
            WebView.SetWebViewClient(GetWebViewClient());
            WebView.Settings.JavaScriptEnabled = true;
            WebView.LoadUrl(GetUrl());

            return view;
        }

        public abstract string GetUrl();
        public virtual WebViewClient GetWebViewClient()
        {
            return new BaseWebViewClient();
        }
    }

    public class BaseWebViewClient : WebViewClient
    {
        public Func<IWebResourceRequest, bool> ShouldOverrideUrlLoadingFunc;
        public Action<WebView, string> OnPageFinishedAction;

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            return ShouldOverrideUrlLoadingFunc == null ? true : ShouldOverrideUrlLoadingFunc.Invoke(request);
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            OnPageFinishedAction?.Invoke(view, url);
        }
    }
}