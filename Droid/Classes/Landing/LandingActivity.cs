using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Xamarin.Facebook;

namespace Slink.Droid
{
    [Activity(MainLauncher = false, Theme = "@style/NoActionBarTheme")]
    public class LandingActivity : BaseActivity
    {
        LoginFragment LoginFragment = new LoginFragment();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var loggedIn = AccessToken.CurrentAccessToken != null;
            if (loggedIn)
            {
               
                StartActivity(typeof(MainActivity));
                return;
            }

            //to remove drawer layout from login screen
            var drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);

            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Add(Resource.Id.under_fragment, new FlyingObjectsFragment());
            transaction.Add(Resource.Id.over_fragment, LoginFragment);
            transaction.Commit();
        }


        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount > 0)
                SupportFragmentManager.PopBackStackImmediate();
            else
                base.OnBackPressed();

            UpdateToolbar();
        }
        public override void UpdateToolbar()
        {
            var resourceId = -1;
            if (SupportFragmentManager.BackStackEntryCount > 0)
                resourceId = Resource.Drawable.abc_ic_ab_back_material;

            SetToolbar(resourceId);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            LoginFragment.CallbackManager.OnActivityResult(requestCode, (int)resultCode, data);
            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}