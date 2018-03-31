using System;
using TPKeyboardAvoiding;
using CoreGraphics;
using Foundation;
using Google.SignIn;
using UIKit;
using System.Threading.Tasks;
using System.Linq;

namespace Slink.iOS
{
    public partial class LoginWithGoogleViewController : BaseLandingViewController, ISignInUIDelegate, ISignInDelegate
    {
        string UserId;

        public LoginWithGoogleViewController(IntPtr handle) : base(handle) { }
        public LoginWithGoogleViewController() : base("LoginWithGoogleViewController") { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RemoveBackBarButtonTitle();

            ImageView.Hidden = true;

            SignIn.SharedInstance.ClientID = "172866788674-grrhf3m9avagoj9bft4o6f53jb0ue1ue.apps.googleusercontent.com";
            SignIn.SharedInstance.SignOutUser();
            SignIn.SharedInstance.UIDelegate = this;
            SignIn.SharedInstance.Delegate = this;
        }
        public override TPKeyboardAvoidingScrollView GetScrollView()
        {
            return ScrollView;
        }

        partial void GoogleSignInButtonClicked(Foundation.NSObject sender)
        {
            SignIn.SharedInstance.SignInUser();
        }
        async public void DidSignIn(SignIn signIn, GoogleUser user, NSError error)
        {
            if (user != null && error == null)
            {
                ShowHud(Strings.Hud.please_wait);

                GoogleNameLabel.Text = user.Profile.Name;
                UserId = user.UserID;

                var outlet = new Outlet();
                outlet.Name = user.Profile.Name;
                outlet.Handle = user.UserID;
                outlet.Type = Outlet.outlet_type_google;
                RealmServices.SaveOutlet(outlet);


                var url = await GoogleAuthenticator.GetProfileURL(UserId);
                ImageView.Hidden = false;
                ImageView.SetImage(url, "NoProfile", "NoProfile", async () =>
                {
                    HideHud();

                    await Task.Delay(TimeSpan.FromSeconds(2));
                    SignIn.SharedInstance.SignOutUser();

                    var popToViewController = NavigationController.ViewControllers.Where(c => c.GetType() == typeof(MyOutletsViewController)).First();
                    NavigationController.PopToViewController(popToViewController, true);

                });
            }
        }


        [Export("signInWillDispatch:error:")]
        public void WillDispatch(SignIn signIn, NSError error) { }

        [Export("signIn:presentViewController:")]
        public void PresentViewController(SignIn signIn, UIViewController viewController)
        {
            PresentViewController(viewController, true, null);
        }

        [Export("signIn:dismissViewController:")]
        public void DismissViewController(SignIn signIn, UIViewController viewController)
        {
            DismissViewController(true, null);
        }
    }
}

