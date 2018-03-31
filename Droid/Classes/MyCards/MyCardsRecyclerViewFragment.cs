using System;

namespace Slink.Droid
{
    public class MyCardsRecyclerViewFragment : RecyclerViewFragment<Card>
    {
        public MyCardsShared Shared = new MyCardsShared();

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            RecyclerViewAdapter.SetListItems(Shared.GetMyCards(true));

            Activity.Title = DrawerShared.navigation_item_my_cards;

            return view;
        }
        public override void RecyclerView_ItemClick(Card obj, int position)
        {
            base.RecyclerView_ItemClick(obj, position);

            if (obj == null)
            {
                var convertedActivity = Activity as BaseActivity;
                if (convertedActivity == null) return;

                var fragment = new NewCardRecyclerViewFragment();
                fragment.Shared.SelectedCard = Card.Create();
                convertedActivity.AddFragmentOver(fragment);
                return;
            }

            if (String.IsNullOrEmpty(obj.Title)) return;

            //convertedActivity.AddFragmentOver();
        }
        public override BaseRecyclerViewAdapter<Card> GetRecyclerViewAdapter()
        {
            var adapter = new MyCardsRecyclerViewAdapter(Activity);
            return adapter;
        }
    }
}
