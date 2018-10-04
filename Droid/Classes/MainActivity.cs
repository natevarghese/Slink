
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.Design.Internal;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;
using Org.Json;
using Slink.Droid.Fragments;
using Xamarin.Facebook;
using Xamarin.Facebook.Core;
using static Google.Ads.AdRequest;

namespace Slink.Droid
{
    [Activity(Label = " ", Theme = "@style/NoActionBarTheme", WindowSoftInputMode = SoftInput.StateHidden | SoftInput.AdjustPan)]
    public class MainActivity : BaseActivity
    {
        AdView mAdView;
        bool ShowsAds = true; //todo not dynamic

        string AdKeyGender = "gender";
        string AdKeyBirthday = "birthday";

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
                    AddFragmentOver(new DiscoverFragment());
                    return;
                }

                if (e.MenuItem.ItemId == Resource.Id.connections)
                {
                    AddFragmentOver(new ConnectionFragment());
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

            mAdView = FindViewById<AdView>(Resource.Id.adView);

            if (ShowsAds)
            {
                var iPersistant = ServiceLocator.Instance.Resolve<IPersistantStorage>();
                var facebookToken = iPersistant.GetFacebookToken();

                var facebookCallback = new FacebookCallback();
                facebookCallback.OnCompletedAction += (GraphResponse obj) =>
                {
                    if (obj == null || obj.RawResponse == null) return;

                    var token = JToken.Parse(obj.RawResponse.ToString());
                    if (token == null) return;

                    var dict = new Dictionary<string, string>();

                    if (token[AdKeyGender] != null)
                        dict.Add(AdKeyGender, token[AdKeyGender].ToString());

                    if (token[AdKeyBirthday] != null)
                        dict.Add(AdKeyBirthday, token[AdKeyBirthday].ToString());

                    ShowBanner(dict);
                };
                var request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, facebookCallback);

                var parameters = new Bundle();
                parameters.PutString("fields", "gender,birthday");
                request.Parameters = parameters;
                request.ExecuteAsync();
            }
        }

        void ShowBanner(Dictionary<string, string> advertisingTargetInfo)
        {
            mAdView.LoadAd(GetRequest(advertisingTargetInfo));
        }
        AdRequest GetRequest(Dictionary<string, string> advertisingTargetInfo)
        {
            var request = new AdRequest.Builder();
            request.AddTestDevice(AdRequest.DeviceIdEmulator);
            request.AddTestDevice("260661DE96DFEDE845160916AD01F3CA"); //samsung tablet

            //Gender
            var gender = Gender.Unknown;
            if (advertisingTargetInfo.ContainsKey(AdKeyGender))
            {
                gender = advertisingTargetInfo[AdKeyGender].Equals("female", StringComparison.InvariantCultureIgnoreCase) ? Gender.Female : Gender.Male;
                request.SetGender((int)gender);
            }

            //Location
            var location = RealmServices.GetLastUserLocation();
            if (location != null)
            {
                var loc = new Android.Locations.Location("");
                loc.Latitude = location.Latitude;
                loc.Longitude = location.Longitude;
                loc.Accuracy = 1;
                request.SetLocation(loc);
            }

            //Birthday
            if (advertisingTargetInfo.ContainsKey(AdKeyBirthday))
            {
                var birthday = DateTime.ParseExact(advertisingTargetInfo[AdKeyBirthday], "MM/dd/yyyy", null);
                var bith = new Java.Util.Date(birthday.Year, birthday.Month, birthday.Day);
                request.SetBirthday(bith);
            }

            return request.Build();
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

    public class FacebookCallback : Java.Lang.Object, GraphRequest.IGraphJSONObjectCallback
    {
        public Action<GraphResponse> OnCompletedAction;

        public void OnCompleted(JSONObject @object, GraphResponse response)
        {
            OnCompletedAction?.Invoke(response);
        }
    }
}
