using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    public class NewOutletFragment : BaseFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.GridView, container, false);

            Activity.Title = Strings.PageTitles.page_title_select_an_outlet;

            var gridView = view.FindViewById<GridView>(Resource.Id.GridView);
            gridView.ItemClick += (sender, e) =>
            {
                var items = RealmServices.GetAllAvailableOutlets();
                var item = items[e.Position];
                if (item == null) return;
                if (!item.AvailbleForAddition)
                {
                    ShowDuplcateAccountAlert("For now, you can only have 1 " + item.Type + " account. We're working on it");
                    return;
                }
                if (item.Type.Equals(Outlet.outlet_type_phone))
                {
                    //NavigationController.PushViewController(new EnterPhoneNumberViewController(), true);
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_email))
                {

                    //NavigationController.PushViewController(new EnterEmailAddressViewController(), true);
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_website))
                {
                    //NavigationController.PushViewController(new WebsiteViewController(), true);
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_facebook))
                {
                    //NavigationController.PushViewController(new LoginWithFacebookViewController(), true);
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_linkedIn))
                {
                    ((BaseActivity)Activity).AddFragmentOver(new LinkedInWebViewFragment());
                    return;
                }


                if (item.Type.Equals(Outlet.outlet_type_github))
                {
                    ((BaseActivity)Activity).AddFragmentOver(new GithubWebViewFragment());
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_instagram))
                {
                    ((BaseActivity)Activity).AddFragmentOver(new InstagramWebViewFragment());
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_google))
                {
                    //NavigationController.PushViewController(new LoginWithGoogleViewController(), true);
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_twitter))
                {
                    ShowTwitterAuthenticationFlow();
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_pinterest))
                {
                    ((BaseActivity)Activity).AddFragmentOver(new PinterestWebViewFragment());
                    return;
                }
            };

            var GridViewAdapter = new NewOutletGridViewAdapter();
            GridViewAdapter.Context = Activity;
            GridViewAdapter.ListItems = RealmServices.GetAllAvailableOutlets();
            gridView.Adapter = GridViewAdapter;

            return view;
        }

        void ShowTwitterAuthenticationFlow()
        {
            var auth = TwitterAuthenticator.GetTwitterAuthenticator((bool sucessful) =>
            {
                if (sucessful)
                {
                    var activity = Activity as BaseActivity;
                    activity?.PopFragmentOverUntil(typeof(MyOutletsRecyclerViewFragment));
                }
            });
            var intent = auth.GetUI(Activity);
            StartActivity(intent);
        }
        void ShowDuplcateAccountAlert(string message)
        {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(Activity);
            builder.SetTitle(Strings.Basic.error);
            builder.SetMessage(message);
            builder.SetCancelable(true);
            builder.SetPositiveButton(Strings.Basic.ok, (senderAlert, args) => { });
            builder.Show();
        }
    }



    class NewOutletGridViewAdapter : BaseAdapter
    {
        public Activity Context;
        public List<Outlet> ListItems;

        public override int Count
        {
            get { return (ListItems == null) ? 0 : ListItems.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }
        public Outlet GetItemInList(int position)
        {
            return ListItems[position];
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? Context.LayoutInflater.Inflate(Resource.Layout.NewOutletCell, null);

            var model = ListItems[position];

            WebImageView imgView = view.FindViewById<WebImageView>(Resource.Id.WebImageView);
            imgView.SetImage(model.RemoteURL, -1, -1, null, WebImageView.DefaultCircleTransformation);

            var textView = view.FindViewById<TextView>(Resource.Id.TextView);
            textView.Text = model.Type;

            return view;
        }
    }

}