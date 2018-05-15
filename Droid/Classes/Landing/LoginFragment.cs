using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;

namespace Slink.Droid
{
    public class LoginFragment : BaseFragment, IFacebookCallback
    {
        LoginShared Shared = new LoginShared();
        public ICallbackManager CallbackManager = CallbackManagerFactory.Create();


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Login, container, false);

            var profileTracker = new FacebookProfileTracker();
            profileTracker.ProfileChanged += (Profile arg1, Profile arg2) =>
            {
                if (arg2 == null) return;

                Shared.FacebookFirstName = arg2.FirstName;
                Shared.FacebookLastName = arg2.LastName;
                Shared.FacebookUserId = arg2.Id;
                Shared.FacebookFullName = arg2.Name;

                Shared.SetUserData(Shared.FacebookUserId, Shared.FacebookFirstName, Shared.FacebookLastName);
            };

            var loginButton = view.FindViewById<LoginButton>(Resource.Id.LoginButton);
            loginButton.SetReadPermissions(Shared.FacebookPermissions.ToArray());
            loginButton.RegisterCallback(CallbackManager, this);

            var privacyPolicyTextView = view.FindViewById<TextView>(Resource.Id.PrivacyPolicyTextView);
            privacyPolicyTextView.Click += (sender, e) =>
            {
                var convertedActivity = Activity as BaseActivity;
                if (convertedActivity == null) return;

                convertedActivity.AddFragmentOver(new PrivacyPolicyFragment());
            };
            return view;
        }

        async public void OnSuccess(Java.Lang.Object result)
        {
            var loginResult = result as LoginResult;
            if (loginResult == null) return;


            string tokenString = AccessToken.CurrentAccessToken.Token;
            if (String.IsNullOrEmpty(tokenString))
            {
                ShowAlert("3", "Token Empty");
                AppCenterManager.Report("3");
                return;
            }

            string userId = loginResult.AccessToken.UserId;


            ShowHud("Loading");

            bool sucessful = await Shared.CreateUser(tokenString);
            if (!sucessful)
            {
                HideHud();
                LoginManager.Instance.LogOut();
                RealmUserServices.Logout();
                AppCenterManager.Report("4");
                return;
            }


            HideHud();
            NextPage();
        }

        public void OnError(FacebookException error) { }
        public void OnCancel() { }

        void ShowAlert(string title, string desc)
        {
            var alert = new Android.Support.V7.App.AlertDialog.Builder(Activity);
            alert.SetTitle(title);
            alert.SetMessage(desc);
            alert.SetPositiveButton(Strings.Basic.ok, (senderAlert, args) => { });
            alert.Show();
        }


        void NextPage()
        {
            var me = RealmUserServices.GetMe(true);

            //cache facebook image
            var str = "https://graph.facebook.com/" + Shared.FacebookUserId + "/picture?type=large";
            var image = ImageUtils.GetImageBitmapFromUrl(str);
            var bytes = ImageUtils.ImagetoByteArray(image, 100);
            S3Utils.UploadPhoto(bytes, me.LocalProfileImageURL, me.RemoteProfileImageURL, "Profile.png", null, null);

            Shared.NextPage();

            if (String.IsNullOrEmpty(me.FirstName) || String.IsNullOrEmpty(me.LastName))
            {
                //((LandingTabbarController)TabBarController).SetSelectedViewControllerByType(typeof(HelloViewController), false, null);
            }
            else
            {
                SlinkUser.SetNextHandelByNameIfNecessary();
                Activity.StartActivity(typeof(MainActivity));
            }
        }
    }

    class FacebookProfileTracker : ProfileTracker
    {
        public Action<Profile, Profile> ProfileChanged;
        protected override void OnCurrentProfileChanged(Profile oldProfile, Profile currentProfile)
        {
            ProfileChanged?.Invoke(oldProfile, currentProfile);
        }
    }
}
