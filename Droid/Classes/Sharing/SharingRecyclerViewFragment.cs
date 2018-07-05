using System;
using Android.Views;

namespace Slink.Droid
{
    public class SharingRecyclerViewFragment : RecyclerViewFragment<SharingShared.Model>
    {
        public SharingShared Shared = new SharingShared();

        public override View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            RecyclerViewAdapter.SetListItems(Shared.GetTableItemsAndroid());

            //Activity.Title = "";

            HasOptionsMenu = true;

            return view;
        }

        public override BaseRecyclerViewAdapter<SharingShared.Model> GetRecyclerViewAdapter()
        {
            var adapter = new SharingRecyclerViewAdapter(Activity);
            return adapter;
        }
        public override void RecyclerView_ItemClick(SharingShared.Model obj, int position)
        {
            base.RecyclerView_ItemClick(obj, position);

        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            var resource = (Shared.SelectedCard == null) ? Resource.Menu.toolbar_save : Resource.Menu.toolbar_delete;
            inflater.Inflate(resource, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.Save:
                    Transporter.SharedInstance.RemoveObject(Transporter.NewOutletAddedTransporterKey);
                    Transporter.SharedInstance.RemoveObject(Transporter.NewOutletAddedValueTransporterKey);

                    //var vc = new NewCardViewController();
                    //(vc.TargetViewController as NewCardTableViewController).Shared.SelectedCard = (TargetViewController as SharingTableViewController).Shared.SelectedCard;
                    //ApplicationExtensions.PushViewController(vc, true);

                    break;
                case Resource.Id.Delete:

                    //((SharingTableViewController)TargetViewController).Shared.DeleteCard();

                    var convertedActiviy = (Activity as BaseActivity);
                    convertedActiviy.HideKeyboard();
                    convertedActiviy.PopFragmentOver();

                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}
