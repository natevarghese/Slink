using System;
using Android.App;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Slink.Droid.Services;

namespace Slink.Droid
{
    public class SharingRecyclerViewAdapter : BaseRecyclerViewAdapter<SharingShared.Model>
    {
        RecyclerView.ViewHolder holder;
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
                    var cell = new CardCell(Context.LayoutInflater.Inflate(Resource.Layout.CardCell, null));
                    return cell;
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
            this.holder = holder;
            var item = GetItemInList(position);

            switch (GetItemViewType(position))
            {
                case 0:
                    ((CardCell)holder).BindDataToView(Context, position, (Card)item.Object, false, false);
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
        public static ImageView imageView { get; set; }
        public TextView TextView { get; set; }
        public int MyPosition { get; set; }
        Context Context;
        public ImageButton bttnShare { get; set; }



        public CardSharingCell(View v) : base(v)
        {
            imageView = v.FindViewById<ImageView>(Resource.Id.ImageView);
            TextView = v.FindViewById<TextView>(Resource.Id.TextView);
            bttnShare = v.FindViewById<ImageButton>(Resource.Id.imageButton);
        }

        public void BindDataToView(Context context, int position, SharingShared.Model model)
        {
            if (model == null) return;
            this.Context = context;
            MyPosition = position;
            //ImageView.SetImageResource(Resource.Drawable.Connections);
            TextView.Text = model.Object.ToString();


            ItemView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                                                                        ViewUtils.DpToPx((Activity)context, 200));



            bttnShare.Click += (sender, e) =>
                {
                    var intent = new Intent(SharingShared.TapToShareBroadCastReceiverClicked);
                    context.SendBroadcast(intent);

                };
            }

    }
      
    }
    


