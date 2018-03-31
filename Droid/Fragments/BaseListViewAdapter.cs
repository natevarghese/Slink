using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    public class BaseListViewAdapter<T> : BaseAdapter where T : new()
    {
        public List<T> ListItems;
        protected Activity Context;

        public BaseListViewAdapter(Activity context) : base()
        {
            Context = context; ;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException();
        }

        public virtual T GetItemInList(int position)
        {
            return ListItems[position];
        }

        public override int Count
        {
            get { return (ListItems == null) ? 0 : ListItems.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            throw new NotImplementedException();
        }

        public void SetListItems(List<T> listItems)
        {
            ListItems = listItems;
            NotifyDataSetChanged();
        }
    }
}
