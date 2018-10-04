using Android.App;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
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
        public ImageView ImageView { get; set; }
        public TextView TextView { get; set; }
        public int MyPosition { get; set; }


        public CardSharingCell(View v) : base(v)
        {
            ImageView = v.FindViewById<ImageView>(Resource.Id.ImageView);
            TextView = v.FindViewById<TextView>(Resource.Id.TextView);
        }

        public void BindDataToView(Context context, int position, SharingShared.Model model)
        {
            if (model == null) return;

            MyPosition = position;
            //ImageView.SetImageResource(Resource.Drawable.Connections);
            TextView.Text = model.Object.ToString();
            ItemView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewUtils.DpToPx((Activity)context, 200));

            if (!ItemView.HasOnClickListeners)
            {
                ItemView.Click += (sender, e) =>
                {
                    var intent = new Intent(SharingShared.TapToShareBroadCastReceiverClicked);
                    context.SendBroadcast(intent);
                    var anim = AnimationUtils.LoadAnimation(context, Resource.Animator.Scale_Share);
                    ImageView.Visibility = ViewStates.Visible;
                    ImageView.StartAnimation(anim);
                    anim.AnimationEnd += Anim_AnimationEnd;
                };


               

            }
        }
        void Anim_AnimationEnd(object sender, Animation.AnimationEndEventArgs e)
        {
            StopAnimation();

        }
        void StopAnimation()
        {
            ImageView.Visibility = ViewStates.Invisible;
            ImageView.ClearAnimation();
        }
    }
}
