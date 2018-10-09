using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Slink.Droid.Services;

namespace Slink.Droid
{
    public class MyCardsRecyclerViewAdapter : BaseRecyclerViewAdapter<Card>
    {
        public MyCardsRecyclerViewAdapter(Activity context) : base(context) { }

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
                    return new CardCell(Context.LayoutInflater.Inflate(Resource.Layout.CardCell, null));
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
                    ((MyCardsFooter)holder).BindDataToView(Context, position, MyCardsShared.CreateNewCard);
                    break;
                case 1:
                    ((CardCell)holder).BindDataToView(Context, position, item, false, true);
                    break;
            }
        }

       
    }

    public class MyCardsFooter : RecyclerView.ViewHolder
    {
        public TextView TextView { get; set; }
        public int MyPosition { get; set; }

        public MyCardsFooter(View v) : base(v)
        {
            TextView = v.FindViewById<TextView>(Resource.Id.TextView);
        }

        public void BindDataToView(Context context, int position, string text)
        {
            MyPosition = position;

            TextView.Text = text;
            TextView.SetTypeface(Typeface.Default, TypefaceStyle.Bold);
            TextView.TextSize = 30;

            ItemView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 300);

            if (ItemView.HasOnClickListeners) return;
            ItemView.Click += (sender, e) =>
            {
                var intent = new Intent(SettingsShared.ItemClickedBroadcastReceiverKey);
                intent.PutExtra(SettingsShared.ItemClickedBroadcastReceiverKeyPosition, MyPosition);
                context.SendBroadcast(intent);
            };
        }
    }


}