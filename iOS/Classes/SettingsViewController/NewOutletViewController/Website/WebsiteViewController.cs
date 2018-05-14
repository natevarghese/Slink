using System;
using TPKeyboardAvoiding;
using Foundation;
using UIKit;
using System.Linq;

namespace Slink.iOS
{
    public partial class WebsiteViewController : BaseLandingViewController
    {
        string URL;

        public WebsiteViewController(IntPtr handle) : base(handle) { }
        public WebsiteViewController() : base("WebsiteViewController") { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RemoveBackBarButtonTitle();

            ActivityIndicatorView.Hidden = true;
            SaveButton.Hidden = true;

            TextField.KeyboardType = UIKeyboardType.Url;
            TextField.Placeholder = "Website URL";
            TextField.AutocorrectionType = UITextAutocorrectionType.No;
            TextField.BecomeFirstResponder();
            TextField.EditingChanged += (sender, e) =>
            {
                WebView.Hidden = true;
                SaveButton.Hidden = true;
                HideActivityIndicatorView();
            };
            TextField.EditingDidEndOnExit += (sender, e) =>
            {
                if (String.IsNullOrEmpty(TextField.Text)) return;

                URL = TextField.Text.Trim();
                if (!URL.Contains("http"))
                {
                    URL = "http://" + URL;
                }

                if (!StringUtils.IsValidURL(URL)) return;

                WebView.Hidden = false;
                WebView.LoadRequest(NSUrlRequest.FromUrl(NSUrl.FromString(URL)));
                ActivityIndicatorView.Hidden = false;
                ActivityIndicatorView.StartAnimating();
            };

            SetProgressButton(SaveButton);

            WebViewSuperViewWidthConstraint.Constant = UIScreen.MainScreen.Bounds.Width - 40;

            WebView.Hidden = true;
            WebView.LoadFinished += (sender, e) =>
            {
                SaveButton.Hidden = false;
                HideActivityIndicatorView();
            };
        }

        public override TPKeyboardAvoidingScrollView GetScrollView()
        {
            return ScrollView;
        }

        partial void SaveButtonClicked(Foundation.NSObject sender)
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

            var popToViewController = fromNewCardPage ? NavigationController.ViewControllers.Where(c => c.GetType() == typeof(NewCardViewController)).First() : NavigationController.ViewControllers.Where(c => c.GetType() == typeof(MyOutletsViewController)).First();
            NavigationController.PopToViewController(popToViewController, true);
        }

        void HideActivityIndicatorView()
        {
            ActivityIndicatorView.Hidden = true;
            ActivityIndicatorView.StopAnimating();
        }
    }
}

