using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;

namespace Slink.Droid
{
    public class BaseRecyclerViewAdapter<T> : RecyclerView.Adapter where T : new()
    {
        public List<T> ListItems;
        protected Activity Context;

        public BaseRecyclerViewAdapter(Activity context) : base() { Context = context; }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) { throw new NotImplementedException(); }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) { throw new NotImplementedException(); }

        public override int GetItemViewType(int position) { return 0; }
        public override int ItemCount { get { return (ListItems == null) ? 0 : ListItems.Count; } }

        public T GetItemInList(int position)
        {
            return ListItems[position];
        }
        public void SetListItems(List<T> listItems)
        {
            ListItems = listItems;
            NotifyDataSetChanged();
        }
    }
}