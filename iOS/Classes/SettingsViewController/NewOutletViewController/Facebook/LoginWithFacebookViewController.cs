using System;
using TPKeyboardAvoiding;
using UIKit;
using Facebook.LoginKit;
using Facebook.CoreKit;
using System.Collections.Generic;

namespace Slink.iOS
{
    public partial class LoginWithFacebookViewController : BaseLandingViewController
    {
        LoginButton loginView;
        ProfilePictureView pictureView;
        List<string> readPermissions = new List<string> { "public_profile", "email", "user_friends" };


        public LoginWithFacebookViewController(IntPtr handle) : base(handle) { }
        public LoginWithFacebookViewController() : base("LoginWithFacebookViewController") { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RemoveBackBarButtonTitle();

            Profile.Notifications.ObserveDidChange((sender, e) =>
            {
                if (e.NewProfile == null)
                    return;

                pictureView.ProfileId = e.NewProfile.UserID;
                FacebookNameLabel.Text = e.NewProfile.Name;

                var outlet = new Outlet();
                outlet.Name = e.NewProfile.Name;
                outlet.Handle = e.NewProfile.UserID;
                outlet.Locked = true;
                outlet.Type = Outlet.outlet_type_facebook;
                RealmServices.SaveOutlet(outlet);

                loginView.RemoveFromSuperview();

            });

            // Set the Read and Publish permissions you want to get
            loginView = new LoginButton(FacebookButtonSuperview.Bounds)
            {
                LoginBehavior = LoginBehavior.Browser,
                ReadPermissions = readPermissions.ToArray()
            };

            // Handle actions once the user is logged in
            loginView.Completed += async (sender, e) =>
            {
                loginView.RemoveFromSuperview();

                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(2));
                NavigationController.PopViewController(true);
            };
            FacebookButtonSuperview.AddSubview(loginView);

            pictureView = new ProfilePictureView(FacebookProfilePictureSuperview.Bounds);
            FacebookProfilePictureSuperview.AddSubview(pictureView);
        }


        public override TPKeyboardAvoidingScrollView GetScrollView()
        {
            return ScrollView;
        }
    }
}

