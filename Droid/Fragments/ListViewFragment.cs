using System;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    public abstract class ListViewFragment<T> : BaseFragment where T : new()
    {
        protected ListView ListView;
        protected BaseListViewAdapter<T> ListViewAdapter;
        protected RelativeLayout BackgroundView;
        protected View EmptyLoadingView, EmptyDataView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ListView, container, false);

            BackgroundView = (RelativeLayout)view.FindViewById(Resource.Id.BackgroundView);

            ListView = (ListView)view.FindViewById(Resource.Id.ListView);
            SetListviewAdapter();

            return view;
        }

        public virtual void IfEmpty(bool endOfFetch)
        {
            bool isTableEmpty = ListView.Adapter.Count == 0;

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



        public abstract BaseListViewAdapter<T> GetListViewAdapter();
        public virtual void SetListviewAdapter()
        {
            ListViewAdapter = GetListViewAdapter();
            ListView.Adapter = ListViewAdapter;
            ListView.ItemClick -= ListView_ItemClick;
            ListView.ItemClick += ListView_ItemClick;
        }
        public virtual void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e) { }

    }
}
