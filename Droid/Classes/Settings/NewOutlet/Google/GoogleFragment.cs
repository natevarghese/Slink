using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    //client id
    //172866788674-at0ramqgpjhq5rmc64to1i1fo939u01q.apps.googleusercontent.com
    public class GoogleFragment : BaseFragment, View.IOnClickListener, GoogleApiClient.IOnConnectionFailedListener
    {
        int RC_SIGN_IN = 9001;

        GoogleApiClient mGoogleApiClient;
        TextView TitleTextView;
        WebImageView ImageView;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Google, container, false);

            TitleTextView = view.FindViewById<TextView>(Resource.Id.TitleTextView);
            ImageView = view.FindViewById<WebImageView>(Resource.Id.ImageView);

            view.FindViewById(Resource.Id.sign_in_button).SetOnClickListener(this);

            // [START configure_signin]
            // Configure sign-in to request the user's ID, email address, and basic
            // profile. ID and basic profile are included in DEFAULT_SIGN_IN.
            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestEmail()
                    .Build();
            // [END configure_signin]

            // [START build_client]
            // Build a GoogleApiClient with access to the Google Sign-In API and the
            // options specified by gso.
            if (mGoogleApiClient == null)
                mGoogleApiClient = new GoogleApiClient.Builder(Activity)
                        .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                        .Build();
            // [END build_client]

            // [START customize_button]
            // Set the dimensions of the sign-in button.
            var signInButton = view.FindViewById<SignInButton>(Resource.Id.sign_in_button);
            signInButton.SetSize(SignInButton.SizeStandard);
            // [END customize_button]

            var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
            if (opr.IsDone)
            {
                // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
                // and the GoogleSignInResult will be available instantly.
                //var result = opr.Get() as GoogleSignInResult;
                //HandleSignInResult(result);

                SignOut();
            }
            else
            {
                // If the user has not previously signed in on this device or the sign-in has expired,
                // this asynchronous branch will attempt to sign in the user silently.  Cross-device
                // single sign-on will occur in this branch.

                ShowHud(Strings.Hud.please_wait);
                opr.SetResultCallback(new SignInResultCallback { Fragment = this });
            }

            return view;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
            if (requestCode == RC_SIGN_IN)
            {
                var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                HandleSignInResult(result);
            }
        }


        async public void HandleSignInResult(GoogleSignInResult result)
        {
            if (result.IsSuccess)
            {
                // Signed in successfully, show authenticated UI.
                var acct = result.SignInAccount;
                TitleTextView.Text = acct.DisplayName;

                ImageView.SetImage(acct.PhotoUrl.ToString(), Resource.Drawable.ic_noprofilewhite, Resource.Drawable.ic_noprofilewhite, "Google", WebImageView.DefaultCircleTransformation);

                var outlet = new Outlet();
                outlet.Name = acct.DisplayName;
                outlet.Handle = acct.Id;
                outlet.Type = Outlet.outlet_type_google;
                RealmServices.SaveOutlet(outlet);

                await Task.Delay(TimeSpan.FromSeconds(2));

                var activity = Activity as BaseActivity;
                activity?.PopFragmentOverUntil(typeof(MyOutletsRecyclerViewFragment));
            }
            else
            {

            }
        }

        void SignIn()
        {
            var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
            StartActivityForResult(signInIntent, RC_SIGN_IN);
        }

        async void SignOut()
        {
            ShowHud(Strings.Hud.loading);

            while (!mGoogleApiClient.IsConnected)
                await Task.Delay(TimeSpan.FromSeconds(1));

            Auth.GoogleSignInApi.SignOut(mGoogleApiClient).SetResultCallback(new SignOutResultCallback { Fragment = this });
        }

        void RevokeAccess()
        {
            Auth.GoogleSignInApi.RevokeAccess(mGoogleApiClient).SetResultCallback(new SignOutResultCallback { Fragment = this });
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            // An unresolvable error has occurred and Google APIs (including Sign-In) will not
            // be available.
        }

        public override void OnStop()
        {
            base.OnStop();
            mGoogleApiClient.Disconnect();
        }


        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.sign_in_button:
                    SignIn();
                    break;
            }
        }

    }



    public class SignInResultCallback : Java.Lang.Object, IResultCallback
    {
        public GoogleFragment Fragment { get; set; }

        public void OnResult(Java.Lang.Object result)
        {
            var googleSignInResult = result as GoogleSignInResult;

            Fragment.HideHud();
            Fragment.HandleSignInResult(googleSignInResult);
        }


    }

    public class SignOutResultCallback : Java.Lang.Object, IResultCallback
    {
        public GoogleFragment Fragment { get; set; }

        public void OnResult(Java.Lang.Object result)
        {
            Fragment.HideHud();
        }
    }
}
