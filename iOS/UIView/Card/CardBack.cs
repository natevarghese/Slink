using Foundation;
using System;
using UIKit;
using ObjCRuntime;

namespace Slink.iOS
{
    public partial class CardBack : UIView, ICardView
    {
        public Action LogoImageButtonAction;

        public CardBack(IntPtr handle) : base(handle) { }

        public static CardBack Create()
        {
            var arr = NSBundle.MainBundle.LoadNib("CardBack", null, null);
            var v = Runtime.GetNSObject<CardBack>(arr.ValueAt(0));

            return v;
        }

        public void BindDataToView(Card card, bool editable)
        {
            if (card == null) return;

            BackgroundColor = ColorUtils.FromHexString(card.BackgroundColor, UIColor.White);

            CompanyNameTextField.Text = card.CompanyName;
            CompanyNameTextField.Placeholder = (editable) ? Strings.Basic.company_name : String.Empty;
            CompanyNameTextField.TextColor = ColorUtils.FromHexString(card.CompanyNameTextColor, UIColor.White);
            CompanyNameTextField.Enabled = editable;
            CompanyNameTextField.RemoveTarget(null, null, UIControlEvent.EditingChanged);
            CompanyNameTextField.EditingChanged += (sender, e) =>
            {
                card.UpdateStringProperty(() => card.CompanyName, CompanyNameTextField.Text.Trim());
                DealWithTextFieldborder(CompanyNameTextField, card, editable);
                NSNotificationCenter.DefaultCenter.PostNotificationName(Strings.InternalNotifications.notification_card_editing_changed, null);
            };
            CompanyNameTextField.RemoveTarget(null, null, UIControlEvent.EditingDidEndOnExit);
            CompanyNameTextField.EditingDidEndOnExit += (sender, e) =>
            {
                CompanyNameTextField.ResignFirstResponder();
            };
            DealWithTextFieldborder(CompanyNameTextField, card, editable);

            var fallback = (editable) ? "Buildings" : null;
            LogoImageButton.SetImageWithCustomCache(card.GetRemoteLogoUrlCached(), fallback, fallback, card.RemoteLogoURL);
            LogoImageButton.UserInteractionEnabled = editable;
        }
        public void ToggleLoadingIndicators(bool visible)
        {
            if (visible)
            {
                LogoImageButton.ShowDimmedView();
                LogoImageButton.ShowLoadingIndicators();
            }
            else
                LogoImageButton.HideLoadingIndicators();
        }
        partial void LogoImageButtonClicked(Slink.iOS.WebImageButton sender)
        {
            LogoImageButtonAction?.Invoke();
        }
        void DealWithTextFieldborder(UITextField field, Card card, bool editable)
        {
            field.BorderStyle = (editable && String.IsNullOrEmpty(field.Text.Trim())) ? UITextBorderStyle.Line : UITextBorderStyle.None;
        }
    }
}