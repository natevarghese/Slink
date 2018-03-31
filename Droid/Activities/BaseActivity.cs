using System;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

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
            var existingFragment = SupportFragmentManager.FindFragmentById(Resource.Id.over_fragment);
            if (existingFragment.GetType() == fragment.GetType()) return;

            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.over_fragment, fragment);
            transaction.AddToBackStack(null);
            transaction.Commit();
            SupportFragmentManager.ExecutePendingTransactions();

            UpdateToolbar();

        }

        public abstract void UpdateToolbar();

        public virtual void Toolbar_NavigationClick(object sender, Android.Support.V7.Widget.Toolbar.NavigationClickEventArgs e)
        {
            OnBackPressed();
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
