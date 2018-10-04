using System;
using Android.Content;
using Android.InputMethodServices;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;

namespace Slink.Droid
{
    public abstract class BaseActivity : AppCompatActivity
    {
        public static string UpdateOverFragmentBroadcastReceiverKey = "Under";
        public static string UpdateOverFragmentBroadcastReceiverKeyFileName = "FileName";

        ActionBroadcastReceiver UpdateOverFragmentBroadcastReceiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            var toolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            UpdateToolbar();
        }

        protected override void OnResume()
        {
            base.OnResume();

            UpdateOverFragmentBroadcastReceiver = new ActionBroadcastReceiver();
            UpdateOverFragmentBroadcastReceiver.NotificationReceived += (obj) =>
            {
                var className = obj.GetStringExtra(UpdateOverFragmentBroadcastReceiverKeyFileName);
                if (String.IsNullOrEmpty(className)) return;

                AddFragmentOver(className);
            };
            RegisterReceiver(UpdateOverFragmentBroadcastReceiver, new IntentFilter(UpdateOverFragmentBroadcastReceiverKey));
        }
        protected override void OnPause()
        {
            base.OnPause();

            if (UpdateOverFragmentBroadcastReceiver != null)
                UnregisterReceiver(UpdateOverFragmentBroadcastReceiver);
        }

        public void SetToolbar(int buttonResource)
        {
            //try
            //{
            var toolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.toolbar);
            toolbar.NavigationClick -= Toolbar_NavigationClick;
            toolbar.NavigationClick += Toolbar_NavigationClick;

            if (buttonResource < 1)
            {
                toolbar.NavigationIcon = null;
                SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            }
            else
            {
                toolbar.SetNavigationIcon(buttonResource);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
        }
        public void AddFragmentOver(string className)
        {
            Type type = Type.GetType("Slink.Droid." + className, true); //todo change to false
            if (type == null) return;

            var newFragment = Activator.CreateInstance(type) as BaseFragment;
            if (newFragment == null) return;

            AddFragmentOver(newFragment);
        }

        public void AddFragmentOver(BaseFragment fragment)
        {
            if (fragment == null) return;

            //dotn replace an the existing fragment with one of the same type. 
            var existingFragment = GetOverFragment();
            if (existingFragment.GetType() == fragment.GetType()) return;

            var tag = fragment.GetType().Name;
            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.over_fragment, fragment);
            transaction.AddToBackStack(tag);
            transaction.Commit();
            SupportFragmentManager.ExecutePendingTransactions();

            UpdateToolbar();

        }
        public void PopFragmentOver()
        {
            SupportFragmentManager.PopBackStack();
        }
        public void PopFragmentOverUntil(Type targetType)
        {
            var existingFragment = GetOverFragment();
            if (existingFragment == null) return;
            if (existingFragment.GetType() == targetType) return;

            SupportFragmentManager.PopBackStackImmediate();
            PopFragmentOverUntil(targetType);
        }
        public Android.Support.V4.App.Fragment GetOverFragment()
        {
            return SupportFragmentManager.FindFragmentById(Resource.Id.over_fragment);
        }
        public void ShowKeyboard(Android.Views.View view)
        {
            new Handler().PostDelayed(() =>
            {
                var service = GetSystemService(Context.InputMethodService) as InputMethodManager;
                service.ShowSoftInput(view, ShowFlags.Implicit);
            }, 200);
        }
        public void HideKeyboard()
        {
            if (CurrentFocus == null) return;
            if (CurrentFocus.WindowToken == null) return;

            var service = GetSystemService(Context.InputMethodService) as InputMethodManager;
            service?.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);
        }
        public abstract void UpdateToolbar();

        public override bool OnKeyDown([GeneratedEnum] Android.Views.Keycode keyCode, KeyEvent e)
        {
            var existingFragment = GetOverFragment() as NewCardRecyclerViewFragment;
            if (keyCode == Android.Views.Keycode.Back && existingFragment != null)
            {
                var saved = existingFragment.SaveCardIfPossible();
                if (!saved)
                    return true;
            }
            return base.OnKeyDown(keyCode, e);
        }
        public virtual void Toolbar_NavigationClick(object sender, Android.Support.V7.Widget.Toolbar.NavigationClickEventArgs e)
        {
            var existingFragment = GetOverFragment() as NewCardRecyclerViewFragment;
            if (existingFragment != null)
            {
                var saved = existingFragment.SaveCardIfPossible();
                if (!saved)
                    return;
            }
            OnBackPressed();
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                UpdateOverFragmentBroadcastReceiver?.Dispose();
                UpdateOverFragmentBroadcastReceiver = null;
            }
        }
    }
}
