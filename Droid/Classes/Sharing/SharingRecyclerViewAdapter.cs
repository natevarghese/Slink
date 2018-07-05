using System;
using Android.App;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    public class SharingRecyclerViewAdapter : BaseRecyclerViewAdapter<SharingShared.Model>
    {
        public SharingRecyclerViewAdapter(Activity context) : base(context) { }

        public override int GetItemViewType(int position)
        {
            var target = GetItemInList(position);

            if (target.IsHeader) return 0;
            if (target.IsFooter) return 2;
            return 1;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case 0:
                    return new CardCell(Context.LayoutInflater.Inflate(Resource.Layout.CardCell, null));
                case 1:
                    return new ImageAndTextViewCell(Context.LayoutInflater.Inflate(Resource.Layout.ImageAndTextViewCell, null));
                case 2:
                    return new CardSharingCell(Context.LayoutInflater.Inflate(Resource.Layout.CardSharingCell, null));
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
                    ((CardCell)holder).BindDataToView(Context, position, (Card)item.Object, true, true);
                    break;
                case 1:
                    ((ImageAndTextViewCell)holder).BindDataToView(Context, position, (Outlet)item.Object);
                    break;
                case 2:
                    ((CardSharingCell)holder).BindDataToView(Context, position, item);
                    break;
            }
        }
    }

    public class CardSharingCell : RecyclerView.ViewHolder
    {
        public WebImageView ImageView { get; set; }
        public TextView TextView { get; set; }
        public int MyPosition { get; set; }

        public CardSharingCell(View v) : base(v)
        {
            ImageView = v.FindViewById<WebImageView>(Resource.Id.ImageView);
            TextView = v.FindViewById<TextView>(Resource.Id.TextView);
        }

        public void BindDataToView(Context context, int position, SharingShared.Model model)
        {
            if (model == null) return;

            MyPosition = position;

            ImageView.SetImageResource(Resource.Drawable.Connections);

            TextView.Text = "Tap to Share";


            ItemView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 200);

            if (ItemView.HasOnClickListeners) return;
            ItemView.Click += (sender, e) =>
            {
                var intent = new Intent(SharingShared.TapToShareBroadCastReceiverClicked);
                context.SendBroadcast(intent);
            };
        }
    }
}
