using System;
using Android.Content;

namespace Slink.Droid
{
    public class MyCardsRecyclerViewFragment : RecyclerViewFragment<Card>
    {
        public MyCardsShared Shared = new MyCardsShared();

        ActionBroadcastReceiver CardClickedBroadcastReceiver, OutletClickedBroadcastReceiver;

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            RecyclerViewAdapter.SetListItems(Shared.GetMyCards(true));

            Activity.Title = DrawerShared.navigation_item_my_cards;

            return view;
        }
        public override void OnResume()
        {
            base.OnResume();


            OutletClickedBroadcastReceiver = new ActionBroadcastReceiver();
            OutletClickedBroadcastReceiver.NotificationReceived += (obj) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    var position = obj.GetIntExtra(MyCardsShared.ItemClickedBroadcastReceiverKeyPosition, -1);
                    if (position == -1) return;

                    var item = RecyclerViewAdapter.GetItemInList(position);

                    RecyclerView_ItemClick(item, position);
                });
            };
            Activity.RegisterReceiver(OutletClickedBroadcastReceiver, new IntentFilter(CardFront.ItemClickedBroadcastReceiverKey));



            CardClickedBroadcastReceiver = new ActionBroadcastReceiver();
            CardClickedBroadcastReceiver.NotificationReceived += (obj) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    var position = obj.GetIntExtra(MyCardsShared.ItemClickedBroadcastReceiverKeyPosition, -1);
                    if (position == -1) return;

                    var item = RecyclerViewAdapter.GetItemInList(position);

                    RecyclerView_ItemClick(item, position);
                });
            };
            Activity.RegisterReceiver(CardClickedBroadcastReceiver, new IntentFilter(MyCardsShared.ItemClickedBroadcastReceiverKeyCardClicked));

        }

        public override void OnPause()
        {
            base.OnPause();

            if (OutletClickedBroadcastReceiver != null)
                Activity.UnregisterReceiver(OutletClickedBroadcastReceiver);

            if (CardClickedBroadcastReceiver != null)
                Activity.UnregisterReceiver(CardClickedBroadcastReceiver);
        }
        public override void RecyclerView_ItemClick(Card obj, int position)
        {
            base.RecyclerView_ItemClick(obj, position);

            var convertedActivity = Activity as BaseActivity;
            if (convertedActivity == null) return;

            if (obj == null)
            {
                var fragment = new NewCardRecyclerViewFragment();
                fragment.Shared.SelectedCard = Card.Create();
                convertedActivity.AddFragmentOver(fragment);
                return;
            }

            var sharingFragment = new SharingRecyclerViewFragment();
            sharingFragment.Shared.SelectedCard = obj;
            convertedActivity.AddFragmentOver(sharingFragment);
        }
        public override BaseRecyclerViewAdapter<Card> GetRecyclerViewAdapter()
        {
            var adapter = new MyCardsRecyclerViewAdapter(Activity);
            return adapter;
        }
    }
}
