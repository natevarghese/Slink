using System;

using Foundation;
using UIKit;
using SDWebImage;
using System.Linq;

namespace Slink.iOS
{
    public partial class MyCardsTableViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("MyCardsTableViewCell");
        public static readonly UINib Nib;
        public static nfloat DefaultHeight = 200;
        public static nfloat MissingNameHeight = 176;

        protected MyCardsTableViewCell(IntPtr handle) : base(handle) { }

        public static MyCardsTableViewCell Create()
        {
            return UINib.FromName("MyCardsTableViewCell", NSBundle.MainBundle).Instantiate(null, null)[0] as MyCardsTableViewCell;
        }

        public void Reset()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;

            ContainerView.Subviews.ToList().ForEach(v => v.RemoveFromSuperview());

            NameLabel.Text = null;
            BackgroundColor = UIColor.Clear;
        }

        public void BindDataToView(Card card, bool showNameLabel, NSIndexPath indexPath)
        {
            Reset();

            if (card == null) return;

            NameLabelHeightConstraint.Constant = (showNameLabel) ? 24 : 0;
            NameLabel.Text = card.Name;

            var cardBack = CardBack.Create();
            cardBack.BindDataToView(card, false);
            cardBack.Hidden = true;
            cardBack.Frame = ContainerView.Bounds;
            ContainerView.AddSubview(cardBack);
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardBack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Top, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardBack, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Right, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardBack, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Bottom, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardBack, NSLayoutAttribute.Left, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Left, 1, 0));

            var cardFront = CardFront.Create();
            cardFront.BindDataToView(card, false, indexPath);
            cardFront.Hidden = true;
            cardFront.Frame = ContainerView.Bounds;
            ContainerView.AddSubview(cardFront);
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardFront, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Top, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardFront, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Right, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardFront, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Bottom, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardFront, NSLayoutAttribute.Left, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Left, 1, 0));

            if (card.IsFlipped)
            {
                cardBack.Hidden = false;
            }
            else
            {
                cardFront.Hidden = false;
            }
        }

        public void Flip(Card card)
        {
            card.Flip();

            var cardFront = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardFront)).First();
            var cardBack = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardBack)).First();

            Transition(ContainerView, 0.5, UIViewAnimationOptions.TransitionFlipFromRight, () =>
            {
                cardFront.Hidden = card.IsFlipped;
                cardBack.Hidden = !card.IsFlipped;
            }, null);
        }
    }
}
