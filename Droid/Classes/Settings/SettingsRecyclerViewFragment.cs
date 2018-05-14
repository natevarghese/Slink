using System;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace Slink.Droid
{
    public class SettingsRecyclerViewFragment : RecyclerViewFragment<SettingsShared.SettingsModel>
    {
        public SettingsShared Shared = new SettingsShared();

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var itemDecorator = new DividerItemDecoration(Activity, DividerItemDecoration.Vertical);
            itemDecorator.SetDrawable(ContextCompat.GetDrawable(Activity, Resource.Drawable.RecyclerViewWhiteDivider));
            RecyclerView.AddItemDecoration(itemDecorator);

            RecyclerViewAdapter.SetListItems(Shared.GetTableItems());

            Activity.Title = DrawerShared.navigation_item_settings;

            return view;
        }
        public override void RecyclerView_ItemClick(SettingsShared.SettingsModel obj, int position)
        {
            base.RecyclerView_ItemClick(obj, position);

            if (obj == null || String.IsNullOrEmpty(obj.Title)) return;

            if (obj.Title.Equals(SettingsShared.navigation_item_my_outlets, StringComparison.InvariantCulture))
            {
                ((MainActivity)Activity).AddFragmentOver(new MyOutletsRecyclerViewFragment());
                return;
            }

            if (obj.Title.Equals(SettingsShared.navigation_item_edit_profile, StringComparison.InvariantCulture))
            {
                ((MainActivity)Activity).AddFragmentOver(new EditProfileFragment());
                return;
            }

            if (obj.Title.Equals(SettingsShared.navigation_item_design, StringComparison.InvariantCulture))
            {
                //Shared.DesignChanged();
                //RecyclerView.GetAdapter().NotifyDataSetChanged();
                return;
            }

            if (obj.Title.Equals(SettingsShared.navigation_item_logout, StringComparison.InvariantCulture))
            {
                RealmUserServices.Logout();

                if (Profile.CurrentProfile != null)
                    LoginManager.Instance.LogOut();

                var iPersistant = ServiceLocator.Instance.Resolve<IPersistantStorage>();
                iPersistant.RemoveAll();

                Activity.StartActivity(typeof(LandingActivity));
                return;
            }
        }
        public override BaseRecyclerViewAdapter<SettingsShared.SettingsModel> GetRecyclerViewAdapter()
        {
            var adapter = new SettingsRecyclerViewAdapter(Activity);
            return adapter;
        }
    }
}