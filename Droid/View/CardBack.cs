using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    [Register("com.nvcomputers.slink.CardBack")]
    public class CardBack : LinearLayout
    {
        Card Card;
        int ParentPosition;

        public CardBack(Context context) : base(context) { }
        public CardBack(Context context, IAttributeSet attributeSet) : base(context, attributeSet) { }
        public CardBack(Context context, IAttributeSet attributeSet, int defStyle) : base(context, attributeSet, defStyle) { }
        public CardBack(IntPtr ptr, JniHandleOwnership handle) : base(ptr, handle) { }

        public void BindDataToView(Card card, bool editable, int parentPosition)
        {
            Card = card;
            ParentPosition = parentPosition;

            SetBackgroundColor(ColorUtils.FromHexString(card.BackgroundColor, Android.Graphics.Color.White));

            var headerImageView = FindViewById<WebImageView>(Resource.Id.LogoImageView);
            headerImageView.SetImage(card.GetRemoteLogoUrlCached(), Resource.Drawable.ic_buildings, Resource.Drawable.ic_buildings, card.RemoteLogoURL, WebImageView.DefaultCircleTransformation);
            headerImageView.Clickable = editable;
            headerImageView.Click -= HeaderImageView_Click;
            if (editable)
                headerImageView.Click += HeaderImageView_Click;

            var companyNameTextView = FindViewById<EditText>(Resource.Id.CompanyNameTextView);
            companyNameTextView.TextChanged -= CompanyNameTextView_TextChanged;
            companyNameTextView.Text = card.CompanyName;
            companyNameTextView.Hint = Strings.Basic.company_name;
            companyNameTextView.Focusable = editable;
            companyNameTextView.Clickable = editable;
            companyNameTextView.SetTextColor(ColorUtils.FromHexString(card.CompanyNameTextColor, Android.Graphics.Color.Black));
            companyNameTextView.TextChanged += CompanyNameTextView_TextChanged;
            if (!editable)
                companyNameTextView.Click += CompanyNameTextView_Click;
            if (!editable)
                companyNameTextView.SetBackgroundColor(Android.Graphics.Color.Transparent); //remove underline

        }

        void HeaderImageView_Click(object sender, EventArgs e)
        {
            var view = sender as View;
            if (view == null) return;

            var intent = new Intent(Strings.InternalNotifications.notification_company_logo_image_clicked);
            view.Context.SendBroadcast(intent);
        }

        void CompanyNameTextView_Click(object sender, EventArgs e)
        {
            var view = sender as View;
            if (view == null) return;

            var intent = new Intent(MyCardsShared.ItemClickedBroadcastReceiverKeyCardClicked);
            intent.PutExtra(MyCardsShared.ItemClickedBroadcastReceiverKeyPosition, ParentPosition);
            view.Context.SendBroadcast(intent);
        }


        void CompanyNameTextView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var view = sender as View;
            if (view == null) return;

            Card.UpdateStringProperty(() => Card.CompanyName, e.Text.ToString().Trim());

            var intent = new Intent(Strings.InternalNotifications.notification_card_editing_changed);
            view.Context.SendBroadcast(intent);
        }
        public WebImageView GetCompanyLogoImageView()
        {
            return FindViewById<WebImageView>(Resource.Id.LogoImageView);
        }
    }
}