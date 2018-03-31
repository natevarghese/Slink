using System;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    public abstract class RecyclerViewFragment<T> : BaseFragment where T : new()
    {
        protected RecyclerView RecyclerView;
        protected BaseRecyclerViewAdapter<T> RecyclerViewAdapter;
        protected RelativeLayout BackgroundView;
        protected View EmptyLoadingView, EmptyDataView;

        ActionBroadcastReceiver ItemClickedBroadcastReceiver;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.RecyclerView, container, false);

            BackgroundView = (RelativeLayout)view.FindViewById(Resource.Id.BackgroundView);

            RecyclerView = (RecyclerView)view.FindViewById(Resource.Id.RecyclerView);
            RecyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            SetListviewAdapter();

            return view;
        }
        public override void OnResume()
        {
            base.OnResume();

            ItemClickedBroadcastReceiver = new ActionBroadcastReceiver();
            ItemClickedBroadcastReceiver.NotificationReceived += (obj) =>
            {
                var position = obj.GetIntExtra(SettingsShared.ItemClickedBroadcastReceiverKeyPosition, -1);
                if (position == -1) return;

                var item = RecyclerViewAdapter.GetItemInList(position);

                RecyclerView_ItemClick(item, position);
            };
            Activity.RegisterReceiver(ItemClickedBroadcastReceiver, new IntentFilter(SettingsShared.ItemClickedBroadcastReceiverKey));

        }
        public override void OnPause()
        {
            base.OnPause();

            if (ItemClickedBroadcastReceiver != null)
                Activity.UnregisterReceiver(ItemClickedBroadcastReceiver);
        }
        public virtual void IfEmpty(bool endOfFetch)
        {
            bool isTableEmpty = RecyclerView.GetAdapter().ItemCount == 0;

            if (!isTableEmpty)
            {
                if (EmptyLoadingView != null && EmptyLoadingView.Parent != null)
                    BackgroundView.RemoveView(EmptyLoadingView);
                if (EmptyDataView != null && EmptyDataView.Parent != null)
                    BackgroundView.RemoveView(EmptyDataView);
                return;
            }

            if (endOfFetch && isTableEmpty)
            {
                if (EmptyLoadingView != null && EmptyLoadingView.Parent != null)
                    BackgroundView.RemoveView(EmptyLoadingView);
                if (EmptyDataView != null && EmptyDataView.Parent != null)
                    BackgroundView.RemoveView(EmptyDataView);
                ShowEmptyDataView();
            }

        }
        public virtual void ShowEmptyLoadingView()
        {
            if (EmptyLoadingView == null) return;

            var layoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            layoutParams.AddRule(LayoutRules.CenterInParent);
            EmptyLoadingView.LayoutParameters = layoutParams;

            BackgroundView.RemoveAllViews();
            BackgroundView.AddView(EmptyLoadingView);
        }

        public virtual void ShowEmptyDataView()
        {
            if (EmptyDataView == null) return;

            var layoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            layoutParams.AddRule(LayoutRules.CenterInParent);
            EmptyDataView.LayoutParameters = layoutParams;

            BackgroundView.RemoveAllViews();
            BackgroundView.AddView(EmptyDataView);
        }



        public abstract BaseRecyclerViewAdapter<T> GetRecyclerViewAdapter();
        public virtual void SetListviewAdapter()
        {
            RecyclerViewAdapter = GetRecyclerViewAdapter();
            RecyclerView.SetAdapter(RecyclerViewAdapter);
        }
        public virtual void RecyclerView_ItemClick(T obj, int position) { }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                ItemClickedBroadcastReceiver?.Dispose();
                ItemClickedBroadcastReceiver = null;
            }
        }
    }
}
