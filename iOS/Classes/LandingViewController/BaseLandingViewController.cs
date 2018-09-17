using System;
using TPKeyboardAvoiding;
using UIKit;

namespace Slink.iOS
{
    abstract public class BaseLandingViewController : BaseViewController
    {
        UIButton ProgressButton;

        public BaseLandingViewController() : base() { }
        public BaseLandingViewController(IntPtr handle) : base(handle) { }
        public BaseLandingViewController(string xibName) : base(xibName) { }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            View.BackgroundColor = UIColor.Clear;

            GetScrollView()?.AddGestureRecognizer(new UITapGestureRecognizer((UITapGestureRecognizer obj) =>
            {
                View.EndEditing(true);
            }));
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            if (GetScrollView() != null)
            {
                for (int i = 0; i < GetScrollView().GestureRecognizers.Length; i++)
                {
                    if (GetScrollView().GestureRecognizers[i].GetType() == typeof(UITapGestureRecognizer))
                    {
                        GetScrollView().RemoveGestureRecognizer(GetScrollView().GestureRecognizers[i]);
                    }
                }
            }
        }
        public abstract TPKeyboardAvoidingScrollView GetScrollView();


        public void SetProgressButton(UIButton progressButton)
        {
            ProgressButton = progressButton;
            ProgressButton.BackgroundColor = ColorUtils.GetColor(Slink.ColorUtils.ColorType.LandingButtonDisabled);
            ProgressButton.Layer.CornerRadius = ProgressButton.Frame.Size.Height / 2;
        }

        protected void ToggleErrorView(bool visible, UIView target, NSLayoutConstraint constraint, nfloat constraintValue, LandingTextField field)
        {
            constraint.Constant = constraintValue;

            if (visible)
            {
                field.SetInvalid();
            }
            else
            {
                field.Reset();
            }

            UIView.Animate(0.25, () =>
            {
                target.Alpha = (visible) ? 1 : 0;
                View.SetNeedsLayout();
            });

            if (ProgressButton != null)
            {
                ProgressButton.BackgroundColor = ValidateAllFields() ? ColorUtils.GetColor(Slink.ColorUtils.ColorType.LandingButton) : UIColor.Gray;
            }
        }

        protected virtual bool ValidateAllFields()
        {
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (ProgressButton != null)
                {
                    ProgressButton.Dispose();
                    ProgressButton = null;
                }
            }
        }
    }
}
