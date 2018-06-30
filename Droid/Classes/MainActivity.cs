
using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.Design.Internal;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    [Activity(Label = " ", Theme = "@style/NoActionBarTheme", WindowSoftInputMode = SoftInput.StateHidden | SoftInput.AdjustPan)]
    public class MainActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Add(Resource.Id.under_fragment, new FlyingObjectsFragment());
            transaction.Add(Resource.Id.over_fragment, new MyCardsRecyclerViewFragment());
            transaction.Commit();

            var footerTextView = FindViewById<TextView>(Resource.Id.footer);
            footerTextView.Text = DrawerShared.GetFooterText();

            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                var drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                drawer?.CloseDrawers();

                if (e.MenuItem.ItemId == Resource.Id.my_cards)
                {
                    AddFragmentOver(new MyCardsRecyclerViewFragment());
                    return;
                }

                if (e.MenuItem.ItemId == Resource.Id.discover)
                {
                    //AddFragmentOver("Discover");
                    return;
                }

                if (e.MenuItem.ItemId == Resource.Id.connections)
                {
                    //AddFragmentOver("Connections");
                    return;
                }

                if (e.MenuItem.ItemId == Resource.Id.settings)
                {
                    AddFragmentOver(new SettingsRecyclerViewFragment());
                    return;
                }
            };

            var itemDecorator = new DividerItemDecoration(this, DividerItemDecoration.Vertical);
            itemDecorator.SetDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.RecyclerViewWhiteDivider));
            var navMenu = (NavigationMenuView)navigationView.GetChildAt(0);
            navMenu.AddItemDecoration(itemDecorator);

            var me = RealmUserServices.GetMe(true);
            var headerView = navigationView.GetHeaderView(0);
            var innititalsTextView = headerView.FindViewById<TextView>(Resource.Id.InnititalsTextView);
            innititalsTextView.Text = (me.FirstName?.FirstOrDefault() + " " + me.LastName?.FirstOrDefault()).Trim();

            var nameTextView = headerView.FindViewById<TextView>(Resource.Id.NameTextView);
            nameTextView.Text = me.Name;

            var handelTextView = headerView.FindViewById<TextView>(Resource.Id.HandelTextView);
            handelTextView.Text = me.Handle;

            UpdateToolbar();
        }


        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount > 0)
                SupportFragmentManager.PopBackStackImmediate();
            else
            {
                var drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                drawer.OpenDrawer((int)GravityFlags.Start);
            }
            UpdateToolbar();
        }
        public override void UpdateToolbar()
        {
            var resourceId = -1;
            if (SupportFragmentManager.BackStackEntryCount > 0)
                resourceId = Resource.Drawable.abc_ic_ab_back_material;
            else
                resourceId = Resource.Drawable.ic_menu_white_24dp;

            SetToolbar(resourceId);
        }
    }
}
