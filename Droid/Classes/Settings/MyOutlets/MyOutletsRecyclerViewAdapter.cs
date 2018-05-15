using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    public class MyOutletsRecyclerViewAdapter : BaseRecyclerViewAdapter<Outlet>
    {
        public MyOutletsRecyclerViewAdapter(Activity context) : base(context) { }

        public override int GetItemViewType(int position)
        {
            var target = GetItemInList(position);
            return target == null ? 0 : 1;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case 0:
                    return new MyCardsFooter(Context.LayoutInflater.Inflate(Resource.Layout.TextViewCell, null));
                case 1:
                    return new ImageAndTextViewCell(Context.LayoutInflater.Inflate(Resource.Layout.ImageAndTextViewCell, null));
                default:
                    return null;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

            var item = GetItemInList(position);

            switch (GetItemViewType(position))
            {
                case 0:
                    ((MyCardsFooter)holder).BindDataToView(Context, position, Strings.TableViewFooters.table_view_footer_create_new_outlet);
                    break;
                case 1:
                    ((ImageAndTextViewCell)holder).BindDataToView(Context, position, item);
                    break;
            }
        }
    }
}