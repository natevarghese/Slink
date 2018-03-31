using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace Slink.iOS
{
	public partial class LandingTextField : UITextField
    {
		public LandingTextField (IntPtr handle) : base (handle){}
		public UIColor PlaceholderColor = UIColor.White;


		public override void Draw(CoreGraphics.CGRect rect)
		{
			BackgroundColor = UIColor.Clear;
			Layer.CornerRadius = Frame.Size.Height / 2;
			Layer.BorderWidth = 1;
			Layer.BorderColor = ColorUtils.GetColor(Slink.ColorUtils.ColorType.LandingTextFieldNormal).CGColor;
			LeftViewMode = UITextFieldViewMode.Always;
			RightViewMode = UITextFieldViewMode.Always;
			LeftView = new UIView(new CGRect(0, 0, 10, 10));
			TextColor = ColorUtils.GetColor(Slink.ColorUtils.ColorType.DefaultText);

			if (SecureTextEntry)
			{
				var showButton = new UIButton(new CGRect(20, 5, 35, rect.Height - 10));
				showButton.SetImage(UIImage.FromBundle("Check"), UIControlState.Normal);
				showButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
				showButton.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				showButton.TouchUpInside -= ShowButton_TouchUpInside;
				showButton.TouchUpInside += ShowButton_TouchUpInside;
				RightView = showButton;
			}
			else
			{
				RightView = new UIView(new CGRect(0, 0, 10, rect.Height));
			}
		}


		public override string Placeholder
		{
			get
			{
				return base.Placeholder;
			}
			set
			{	if (!String.IsNullOrEmpty(value))
				{
					AttributedPlaceholder = new NSAttributedString(value, new UIStringAttributes { ForegroundColor = PlaceholderColor  });
				}
				else {
					base.Placeholder = value;
				}
			}
		}
		public override bool SecureTextEntry
		{
			get
			{
				return base.SecureTextEntry;
			}
			set
			{
				base.SecureTextEntry = value;
				SetNeedsLayout();
			}
		}

		void ShowButton_TouchUpInside (object sender, EventArgs e)
		{
			SecureTextEntry = !SecureTextEntry;

			var button = sender as UIButton;
			if (button == null) return;

			button.SetImage(UIImage.FromBundle((SecureTextEntry) ? "Check" : "X"), UIControlState.Normal);
		}

		public void SetInvalid()
		{
			Layer.BorderColor = ColorUtils.GetColor(Slink.ColorUtils.ColorType.LandingTextFieldError).CGColor;
		}

		public void Reset()
		{
			Layer.BorderColor = ColorUtils.GetColor(Slink.ColorUtils.ColorType.LandingTextFieldNormal).CGColor;
		}


		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				if (PlaceholderColor != null)
				{
					PlaceholderColor.Dispose();
					PlaceholderColor = null;
				}
			}
		}
    }
}