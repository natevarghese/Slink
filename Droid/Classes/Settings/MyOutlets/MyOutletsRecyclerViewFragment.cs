using System;
using System.Linq;
using Android.Views;

namespace Slink.Droid
{
    public class MyOutletsRecyclerViewFragment : RecyclerViewFragment<Outlet>
    {
        public Action<Outlet> OutletSelected;

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            RecyclerViewAdapter.SetListItems(RealmServices.GetMyOutlets().ToList());

            Activity.Title = SettingsShared.navigation_item_my_outlets;


            RegisterForContextMenu(RecyclerView);

            return view;
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var position = item.Order;

            var model = RecyclerViewAdapter.GetItemInList(position);
            if (model == null) return base.OnContextItemSelected(item);

            switch (item.TitleFormatted.ToString().ToLower())
            {
                case "delete":
                    RealmServices.DeleteOutlet(model);
                    RecyclerViewAdapter.SetListItems(RealmServices.GetMyOutlets().ToList());
                    break;
                default: break;
            }

            return base.OnContextItemSelected(item);
        }
        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
        }
        public override void RecyclerView_ItemClick(Outlet obj, int position)
        {
            base.RecyclerView_ItemClick(obj, position);

            if (obj == null)
            {
                var convertedActivity = Activity as BaseActivity;
                if (convertedActivity == null) return;

                var fragment = new NewOutletFragment();
                convertedActivity.AddFragmentOver(fragment);
                return;
            }

            OutletSelected?.Invoke(obj);
        }
        public override BaseRecyclerViewAdapter<Outlet> GetRecyclerViewAdapter()
        {
            var adapter = new MyOutletsRecyclerViewAdapter(Activity);
            return adapter;
        }
    }
}
