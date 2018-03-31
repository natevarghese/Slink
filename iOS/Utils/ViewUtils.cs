using System;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public static class ViewUtils
    {
        public static UIVisualEffectView CreateMask(string message, UIView view, bool showActivityIndicator)
        {
            UIVisualEffectView mask = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark));
            mask.BackgroundColor = UIColor.FromWhiteAlpha(0, (nfloat).1);
            mask.Frame = view.Frame;
            mask.Tag = 100;

            if (showActivityIndicator)
            {
                UIActivityIndicatorView activityIndicatorView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
                activityIndicatorView.Center = mask.Center;
                activityIndicatorView.StartAnimating();
                activityIndicatorView.Tag = 100;
                mask.AddSubview(activityIndicatorView);
            }

            UILabel label = new UILabel(new CoreGraphics.CGRect(0, 100, view.Frame.Size.Width, 40));
            label.Text = message;
            label.Font = UIFont.SystemFontOfSize(20);
            label.MinimumScaleFactor = 9 / 20;
            label.AdjustsFontSizeToFitWidth = true;
            label.TextColor = UIColor.White;
            label.TextAlignment = UITextAlignment.Center;
            label.Tag = 101;

            mask.AddSubview(label);

            return mask;
        }
        public static UIImageView ImageByRenderingView(UIView view)
        {
            UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, false, 0);
            view.DrawViewHierarchy(view.Bounds, true);
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            var imageView = new UIImageView(image);
            return imageView;
        }
    }
}

