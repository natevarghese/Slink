using System;
using Foundation;
using UIKit;
using Facebook.LoginKit;
using Facebook.CoreKit;

using System.Collections.Generic;
using System.Threading;
using Realms.Sync;

namespace Slink.iOS
{
    public partial class LoginViewController : BaseLandingViewController
    {
        LoginShared Shared = new LoginShared();
        LoginButton loginView;

        CancellationTokenSource LogoutCancellationToken;

        public LoginViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            Profile.Notifications.ObserveDidChange((sender, e) =>
            {
                if (e.NewProfile == null)
                    return;

                Shared.FacebookFirstName = e.NewProfile.FirstName;
                Shared.FacebookLastName = e.NewProfile.LastName;
                Shared.FacebookUserId = e.NewProfile.UserID;
                Shared.FacebookFullName = e.NewProfile.Name;

                Shared.SetUserData(Shared.FacebookUserId, Shared.FacebookFirstName, Shared.FacebookLastName);
            });
            // Set the Read and Publish permissions you want to get
            loginView = new LoginButton(FacebookButtonSuperview.Bounds)
            {
                LoginBehavior = LoginBehavior.Native,
                ReadPermissions = Shared.FacebookPermissions.ToArray()
            };

            // Handle actions once the user is logged in
            loginView.Completed += async (sender, e) =>
            {
                if (e == null) return;
                if (e.Result != null && e.Result.IsCancelled) return;

                if (e.Error != null)
                {
                    AppCenterManager.Report(e.Error.LocalizedDescription);
                    AppCenterManager.Report("2");
                    ShowAlert("Oops!", e.Error.LocalizedDescription);
                    return;
                }
                //427856514
                if (e.Result.Token == null) return;
                string tokenString = e.Result.Token.TokenString;

                if (String.IsNullOrEmpty(tokenString))
                {
                    ShowAlert("3", "Token Empty");
                    AppCenterManager.Report("3");
                    return;
                }

                ShowHud("Loading");

                //timmmys token
                //tokenString = "EAAEBJG2Ozg4BAEN1geU5bEXTn3CazQfnI2CWndDmdZBvUDPSfbPh8WZB1ZBXqLuZCdRjhsT3fJMriNY7qmZAoL2eTWxOOOimZBwjDOLGChRJ9NMaMYjJF5cLnAZClX9bg6VtdpPXaXKpyhZAZAaUcRLZCBgJJ3X5zcMEtd3BLKAqpDDRZA0SFWPMAPaNinZA53sv1zmZAvkCSaQD3CZBD4IWq96yrOnZASHaWdADuUsvzC5ZC5LgpNpKoRhXP1I0";


                bool sucessful = await Shared.CreateUser(tokenString);
                if (!sucessful)
                {
                    HideHud();
                    new LoginManager().LogOut();
                    RealmUserServices.Logout();
                    AppCenterManager.Report("4");
                    return;
                }


                HideHud();
                NextPage();
            };

            // Handle actions once the user is logged out
            loginView.LoggedOut += (s, ev) =>
            {
                if (LogoutCancellationToken == null)
                {
                    LogoutCancellationToken = new CancellationTokenSource();
                    SettingsShared.Logout(LogoutCancellationToken.Token);
                }
            };
            FacebookButtonSuperview.AddSubview(loginView);
        }
        public override TPKeyboardAvoiding.TPKeyboardAvoidingScrollView GetScrollView() { return null; }

        void ShowAlert(string title, string desc)
        {
            UIAlertController AlertController = UIAlertController.Create(title, desc, UIAlertControllerStyle.Alert);
            AlertController.AddAction(UIAlertAction.Create(Strings.Basic.ok, UIAlertActionStyle.Default, null));
            PresentViewController(AlertController, true, null);
        }

        void NextPage()
        {
            var me = RealmUserServices.GetMe(true);
            var realm = RealmManager.SharedInstance.GetRealm(null);

            //cache facebook image
            var str = "https://graph.facebook.com/" + Shared.FacebookUserId + "/picture?type=large";
            var image = UIImage.LoadFromData(NSData.FromUrl(NSUrl.FromString(str)));
            var bytes = ImageUtils.ByteArrayFromImage(image, 50);
            S3Utils.UploadPhoto(bytes, me.LocalProfileImageURL, me.RemoteProfileImageURL, "Profile.png", null, null);

            Shared.NextPage();

            UIView.Animate(1, delegate
            {
                View.Alpha = 0;
            }, delegate
            {

                if (String.IsNullOrEmpty(me.FirstName) || String.IsNullOrEmpty(me.LastName))
                {
                    ((LandingTabbarController)TabBarController).SetSelectedViewControllerByType(typeof(HelloViewController), false, null);
                }
                else
                {
                    SlinkUser.SetNextHandelByNameIfNecessary();
                    ApplicationExtensions.ShowOnboarding(false);
                }
            });
        }

        partial void TermsOfServiceClicked(Foundation.NSObject sender)
        {
            Transporter.SharedInstance.SetObject("URL", Strings.SystemUrls.privacy_policy);
            Transporter.SharedInstance.SetObject("Title", "Privacy Policy");
            ((LandingTabbarController)TabBarController).SetSelectedViewControllerByType(typeof(WebViewViewController), true, null);
        }
    }
}

