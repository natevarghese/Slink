using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    [Register("com.nvcomputers.slink.CardFront")]
    public class CardFront : LinearLayout
    {
        public static string ItemClickedBroadcastReceiverKey = "OutletClicked";
        public static string ItemClickedBroadcastReceiverKeyPosition = "Position";

        Card Card;
        //bool Editable;
        int ParentPosition;

        public CardFront(Context context) : base(context) { }
        public CardFront(Context context, IAttributeSet attributeSet) : base(context, attributeSet) { }
        public CardFront(Context context, IAttributeSet attributeSet, int defStyle) : base(context, attributeSet, defStyle) { }
        public CardFront(IntPtr ptr, JniHandleOwnership handle) : base(ptr, handle) { }

        public void BindDataToView(Card card, bool editable, int parentPosition)
        {
            Card = card;
            // Editable = editable;
            ParentPosition = parentPosition;

            var backgroundView = FindViewById<RelativeLayout>(Resource.Id.BackgroundView);
            backgroundView.SetBackgroundColor(Android.Graphics.Color.Red);
            //ColorUtils.FromHexString(Card.BorderColor, Android.Graphics.Color.White);

            var headerImageView = FindViewById<WebImageView>(Resource.Id.HeaderImageView);
            headerImageView.SetImage(Card.GetRemoteHeaderUrlCached(), Resource.Drawable.ic_noprofile, Resource.Drawable.ic_noprofile, Card.RemoteHeaderURL, WebImageView.DefaultCircleTransformation);
            headerImageView.Clickable = editable;
            headerImageView.Click -= HeaderImageView_Click;
            if (editable)
                headerImageView.Click += HeaderImageView_Click;

            var viewWidth = Context.Resources.DisplayMetrics.WidthPixels - 20;
            var height = viewWidth / 4;
            var lp = new RelativeLayout.LayoutParams(height, height);
            headerImageView.LayoutParameters = lp;

            var userDisplayNameTextView = FindViewById<EditText>(Resource.Id.UserDisplayNameTextView);
            userDisplayNameTextView.TextChanged -= UserDisplayNameTextView_TextChanged;
            userDisplayNameTextView.Click -= UserDisplayNameTextView_Click;
            userDisplayNameTextView.Text = Card.UserDisplayName;
            userDisplayNameTextView.Focusable = editable;
            userDisplayNameTextView.Clickable = editable;
            userDisplayNameTextView.TextChanged += UserDisplayNameTextView_TextChanged;
            if (!editable)
                userDisplayNameTextView.Click += UserDisplayNameTextView_Click;
            if (!editable)
                userDisplayNameTextView.SetBackgroundColor(Android.Graphics.Color.Transparent); //remove underline

            var titleTextField = FindViewById<EditText>(Resource.Id.TitleTextField);
            titleTextField.TextChanged -= TitleTextField_TextChanged;
            titleTextField.Click -= UserDisplayNameTextView_Click;
            titleTextField.Text = Card.Title;
            titleTextField.Focusable = editable;
            titleTextField.Clickable = editable;
            titleTextField.TextChanged += TitleTextField_TextChanged;
            if (!editable)
                titleTextField.Click += TitleTextField_Click;
            if (!editable)
                titleTextField.SetBackgroundColor(Android.Graphics.Color.Transparent); //remove underline

            var recyclerView = FindViewById<RecyclerView>(Resource.Id.RecyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(Context, LinearLayoutManager.Horizontal, false));

            var adapter = new CardFrontRecyclerViewAdaper((Activity)Context);
            adapter.Editable = editable;
            adapter.SetListItems(Card.Outlets.ToList());
            adapter.ParentPosition = parentPosition;
            recyclerView.SetAdapter(adapter);


            var noOutletsTextView = FindViewById<TextView>(Resource.Id.NoOutletsTextView);
            noOutletsTextView.Click -= NoOutletsTextView_Click;
            noOutletsTextView.Visibility = (Card.Outlets == null || Card.Outlets.Count == 0) ? ViewStates.Visible : ViewStates.Gone;
            noOutletsTextView.Click += NoOutletsTextView_Click;
        }

        void HeaderImageView_Click(object sender, EventArgs e)
        {
            var view = sender as View;
            if (view == null) return;

            var intent = new Intent(Strings.InternalNotifications.notification_card_user_image_clicked);
            view.Context.SendBroadcast(intent);
        }

        void NoOutletsTextView_Click(object sender, EventArgs e)
        {
            var view = sender as View;
            if (view == null) return;

            var intent = new Intent(Strings.InternalNotifications.notification_no_outlets_tapped);
            view.Context.SendBroadcast(intent);
        }

        void TitleTextField_Click(object sender, EventArgs e)
        {
            var view = sender as View;
            if (view == null) return;

            var intent = new Intent(MyCardsShared.ItemClickedBroadcastReceiverKeyCardClicked);
            intent.PutExtra(MyCardsShared.ItemClickedBroadcastReceiverKeyPosition, ParentPosition);
            view.Context.SendBroadcast(intent);
        }


        void TitleTextField_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var view = sender as View;
            if (view == null) return;

            Card.UpdateStringProperty(() => Card.Title, e.Text.ToString().Trim());

            var intent = new Intent(Strings.InternalNotifications.notification_card_editing_changed);
            intent.PutExtra(MyCardsShared.ItemClickedBroadcastReceiverKeyPosition, ParentPosition);
            view.Context.SendBroadcast(intent);
        }

        void UserDisplayNameTextView_Click(object sender, EventArgs e)
        {
            var view = sender as View;
            if (view == null) return;

            var intent = new Intent(MyCardsShared.ItemClickedBroadcastReceiverKeyCardClicked);
            intent.PutExtra(MyCardsShared.ItemClickedBroadcastReceiverKeyPosition, ParentPosition);
            view.Context.SendBroadcast(intent);
        }


        void UserDisplayNameTextView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var view = sender as View;
            if (view == null) return;

            Card.UpdateStringProperty(() => Card.UserDisplayName, e.Text.ToString().Trim());

            var intent = new Intent(Strings.InternalNotifications.notification_card_editing_changed);
            view.Context.SendBroadcast(intent);
        }


        public WebImageView GetUserImageView()
        {
            return FindViewById<WebImageView>(Resource.Id.HeaderImageView);
        }
    }

    class CardFrontRecyclerViewAdaper : BaseRecyclerViewAdapter<Outlet>
    {
        public int ParentPosition;
        public bool Editable;

        public CardFrontRecyclerViewAdaper(Activity context) : base(context) { }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = Context.LayoutInflater.Inflate(Resource.Layout.ImageViewCell, null);

            //make sure its square
            itemView.SetMinimumHeight(parent.MeasuredHeight);
            itemView.SetMinimumWidth(parent.MeasuredHeight);

            return new ImageViewCell(itemView, parent.MeasuredHeight);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = GetItemInList(position);
            ((ImageViewCell)holder).BindDataToView(Context, Editable ? position : ParentPosition, item);
        }
    }

    public class ImageViewCell : RecyclerView.ViewHolder
    {
        WebImageView ImageView;

        public ImageViewCell(View v, int height) : base(v)
        {
            ImageView = v.FindViewById<WebImageView>(Resource.Id.WebImageView);
            ImageView.SetMinimumHeight(height);
            ImageView.SetMinimumWidth(height);
        }

        public void BindDataToView(Context context, int position, Outlet model)
        {
            if (model == null) return;

            ImageView.SetImage(model.RemoteURL, -1, -1, null, WebImageView.DefaultCircleTransformation);

            if (!ItemView.HasOnClickListeners)
            {
                ItemView.Click += (sender, e) =>
                {
                    var key = CardFront.ItemClickedBroadcastReceiverKey;
                    var intent = new Intent(key);
                    intent.PutExtra(SharingShared.ItemClickedBroadcastReceiverKeyPosition, position);
                    Transporter.SharedInstance.SetObject("Outlet", model);
                    context.SendBroadcast(intent);
                };
            }
        }
    }
}
