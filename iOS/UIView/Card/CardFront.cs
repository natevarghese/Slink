using Foundation;
using System;
using ObjCRuntime;
using UIKit;
using System.Linq;
using CoreGraphics;

namespace Slink.iOS
{
    public partial class CardFront : UIView, ICardView
    {
        public Action HeaderImageButtonAction;
        UITapGestureRecognizer CollectionViewTapGesture;

        public CardFront(IntPtr handle) : base(handle) { }

        public static CardFront Create()
        {
            var arr = NSBundle.MainBundle.LoadNib("CardFront", null, null);
            var v = Runtime.GetNSObject<CardFront>(arr.ValueAt(0));

            return v;
        }

        public void BindDataToView(Card card, bool editable, NSIndexPath indexPath)
        {
            if (card == null) return;

            BackgroundColor = ColorUtils.FromHexString(card.BorderColor, UIColor.White);

            UserDisplayNameTextField.Text = card.UserDisplayName;
            UserDisplayNameTextField.Placeholder = (editable) ? Strings.Basic.your_name : String.Empty;
            UserDisplayNameTextField.Enabled = editable;
            UserDisplayNameTextField.RemoveTarget(null, null, UIControlEvent.EditingChanged);
            UserDisplayNameTextField.EditingChanged += (sender, e) =>
            {
                card.UpdateStringProperty(() => card.UserDisplayName, UserDisplayNameTextField.Text.Trim());
                DealWithTextFieldborder(UserDisplayNameTextField, card, editable);
                NSNotificationCenter.DefaultCenter.PostNotificationName(Strings.InternalNotifications.notification_card_editing_changed, null);
            };
            UserDisplayNameTextField.RemoveTarget(null, null, UIControlEvent.EditingDidEndOnExit);
            UserDisplayNameTextField.EditingDidEndOnExit += (sender, e) =>
            {
                UserDisplayNameTextField.ResignFirstResponder();
            };
            DealWithTextFieldborder(UserDisplayNameTextField, card, editable);

            TitleTextField.Text = card.Title;
            TitleTextField.Enabled = editable;
            TitleTextField.Placeholder = (editable) ? Strings.Basic.title : String.Empty;
            TitleTextField.RemoveTarget(null, null, UIControlEvent.EditingChanged);
            TitleTextField.EditingChanged += (sender, e) =>
            {
                card.UpdateStringProperty(() => card.Title, TitleTextField.Text.Trim());
                DealWithTextFieldborder(TitleTextField, card, editable);
                NSNotificationCenter.DefaultCenter.PostNotificationName(Strings.InternalNotifications.notification_card_editing_changed, null);
            };
            TitleTextField.RemoveTarget(null, null, UIControlEvent.EditingDidEndOnExit);
            TitleTextField.EditingDidEndOnExit += (sender, e) =>
            {
                TitleTextField.ResignFirstResponder();
            };
            DealWithTextFieldborder(TitleTextField, card, editable);

            HeaderImageButton.SetImageWithCustomCache(card.GetRemoteHeaderUrlCached(), "NoProfile", "NoProfile", card.RemoteHeaderURL);
            HeaderImageButton.Layer.MasksToBounds = true;
            HeaderImageButton.Layer.CornerRadius = HeaderImageButton.Frame.Size.Width / 2;
            HeaderImageButton.ClipsToBounds = false;
            HeaderImageButton.UserInteractionEnabled = editable;

            CollectionView.RegisterNibForCell(UINib.FromName("MyCardsCollectionViewCell", NSBundle.MainBundle), MyCardsCollectionViewCell.Key);
            CollectionView.ContentInset = new UIEdgeInsets(0, 8, 0, 8);
            CollectionView.WeakDataSource = new MyCardsTableViewCellCollectionViewDataSource(card);
            CollectionView.WeakDelegate = new MyCardsTableViewCellCollectionViewDelegateFlowLayout();

            if (CollectionViewTapGesture == null)
            {
                CollectionViewTapGesture = new UITapGestureRecognizer((obj) =>
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName(Strings.InternalNotifications.notification_collection_view_tapped, indexPath);
                });
            }
            CollectionView.RemoveGestureRecognizer(CollectionViewTapGesture);
            CollectionView.AddGestureRecognizer(CollectionViewTapGesture);

            NoOutletsButton.Hidden = !(card.Outlets == null || card.Outlets.Count == 0);
        }
        public void ToggleLoadingIndicators(bool visible)
        {
            if (visible)
            {
                HeaderImageButton.ShowDimmedView();
                HeaderImageButton.ShowLoadingIndicators();
            }
            else
                HeaderImageButton.HideLoadingIndicators();
        }
        void DealWithTextFieldborder(UITextField field, Card card, bool editable)
        {
            field.BorderStyle = (editable && String.IsNullOrEmpty(field.Text.Trim())) ? UITextBorderStyle.Line : UITextBorderStyle.None;
        }
        partial void NoOutletsButtonClicked(Foundation.NSObject sender)
        {
            NSNotificationCenter.DefaultCenter.PostNotificationName(Strings.InternalNotifications.notification_no_outlets_tapped, null);
        }
        partial void HeaderImageButtonClicked(Slink.iOS.WebImageButton sender)
        {
            HeaderImageButtonAction?.Invoke();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                CollectionViewTapGesture?.Dispose();
                CollectionViewTapGesture = null;
            }
        }

        #region Used For Animation
        public void AnimateTextUserNameLabel(string text, double delay, Action completed)
        {
            UserDisplayNameTextField.AnimateText(text, delay, completed);
        }
        public void AddUserImage(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                HeaderImageButton.SetImage(null, new UIControlState());
            }
            else
            {
                HeaderImageButton.SetImage(url, "NoProfile", "NoProfile");
            }
        }
        public void ChangeBorderColor(UIColor color)
        {
            BackgroundColor = color;
        }
        public void AddEmptyPhoneNumber()
        {
            CollectionView.InsertItems(new NSIndexPath[] { NSIndexPath.FromRowSection(0, 0) });
            NoOutletsButton.Hidden = true;
        }
        public void AddEmptySocialMedia(int count)
        {
            var paths = new NSIndexPath[count - 1];
            for (int i = 1; i < count; i++)
            {
                paths[i - 1] = NSIndexPath.FromRowSection(i, 0);
            }

            CollectionView.ReloadSections(new NSIndexSet(0));
            NoOutletsButton.Hidden = true;
        }

        #endregion
    }

    #region UICollectionViewDataSource
    class MyCardsTableViewCellCollectionViewDataSource : UICollectionViewDataSource
    {
        Card Card;

        public MyCardsTableViewCellCollectionViewDataSource(Card card)
        {
            Card = card;
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return (Card.Outlets == null) ? 0 : 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return (Card.Outlets == null) ? 0 : Card.Outlets.Where(c => !c.Omitted).Count();
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (MyCardsCollectionViewCell)collectionView.DequeueReusableCell(MyCardsCollectionViewCell.Key, indexPath);
            var item = Card.Outlets.Where(c => !c.Omitted).OrderBy(c => c.Type).ElementAt(indexPath.Row);

            cell.BindDataToView(item);
            return cell;
        }

    }
    #endregion

    class MyCardsTableViewCellCollectionViewDelegateFlowLayout : UICollectionViewDelegateFlowLayout
    {
        public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            return new CGSize(collectionView.Frame.Size.Height, collectionView.Frame.Size.Height);
        }
    }
}