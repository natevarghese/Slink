using System;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Android.Text;

namespace Slink.Droid
{
    public class WebsiteFragment : BaseFragment
    {
        WebView WebView;
        Button ProgressButton;
        ProgressBar ProgressBar;
        EditText TextField;
        string URL;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Website, container, false);

            Activity.Title = "Website";

            CookieManager.Instance.RemoveAllCookies(null);
            CookieManager.Instance.Flush();

            WebView = view.FindViewById<WebView>(Resource.Id.WebView);
            WebView.Visibility = ViewStates.Invisible;
            WebView.ClearHistory();
            WebView.ClearCache(true);

            var client = new BaseWebViewClient();
            client.ShouldOverrideUrlLoadingFunc += (arg) => { return false; };
            client.OnPageFinishedAction += (arg1, arg2) =>
            {
                ProgressBar.Visibility = ViewStates.Invisible;

                ProgressButton.Text = "Save";
            };
            WebView.SetWebViewClient(client);

            TextField = view.FindViewById<EditText>(Resource.Id.WebsiteTextField);
            TextField.Hint = "Website URL";
            TextField.InputType = InputTypes.TextVariationWebEditText | InputTypes.TextFlagNoSuggestions;
            TextField.TextChanged += (sender, e) =>
            {
                WebView.Visibility = ViewStates.Invisible;
                ProgressBar.Visibility = ViewStates.Invisible;
                ProgressButton.Text = "Validate";
            };

            ProgressButton = view.FindViewById<Button>(Resource.Id.ProgressButton);
            ProgressButton.Click += (sender, e) =>
            {
                if (String.IsNullOrEmpty(TextField.Text)) return;

                var activity = Activity as BaseActivity;

                if (ProgressButton.Text.Equals("Save", StringComparison.InvariantCultureIgnoreCase))
                {
                    string name = null;
                    if (URL.Contains("http://"))
                    {
                        name = URL.Substring(7);
                    }
                    else if (URL.Contains("https://"))
                    {
                        name = URL.Substring(8);
                    }


                    var outlet = new Outlet();
                    outlet.Name = name;
                    outlet.Handle = URL;
                    outlet.Type = Outlet.outlet_type_website;
                    RealmServices.SaveOutlet(outlet);

                    var fromNewCardPage = false;
                    if (Transporter.SharedInstance.ContainsKey(Transporter.NewOutletAddedTransporterKey))
                        fromNewCardPage = (bool)Transporter.SharedInstance.GetObjectForKey(Transporter.NewOutletAddedTransporterKey);

                    //var popToViewController = fromNewCardPage ? NavigationController.ViewControllers.Where(c => c.GetType() == typeof(NewCardViewController)).First() : NavigationController.ViewControllers.Where(c => c.GetType() == typeof(MyOutletsViewController)).First();
                    activity?.PopFragmentOverUntil(typeof(MyOutletsRecyclerViewFragment));

                    return;
                }


                URL = TextField.Text.Trim();
                if (!URL.Contains("http"))
                {
                    URL = "http://" + URL;
                }

                if (!StringUtils.IsValidURL(URL)) return;

                activity?.HideKeyboard();

                WebView.Visibility = ViewStates.Visible;
                WebView.LoadUrl(URL);
                ProgressBar.Visibility = ViewStates.Visible;
            };

            ProgressBar = view.FindViewById<ProgressBar>(Resource.Id.ProgressBar);
            ProgressBar.Visibility = ViewStates.Invisible;

            return view;
        }


        public override void OnDestroy()
        {
            base.OnDestroy();

            WebView?.Dispose();
            WebView = null;

            ProgressButton?.Dispose();
            ProgressButton = null;

            ProgressBar?.Dispose();
            ProgressBar = null;

            TextField?.Dispose();
            TextField = null;

            URL = null;
        }
    }
}