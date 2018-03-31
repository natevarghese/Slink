using Foundation;
using System;
using UIKit;
using SDWebImage;

namespace Slink.iOS
{
    public partial class WebImage : UIImageView
    {
        UIActivityIndicatorView ActivityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);

        public WebImage(IntPtr handle) : base(handle) { }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);

            AddSubview(ActivityIndicator);
        }

        public void SetImage(string url, string placeholderBundleFileName, string fallbackBundleFileName, Action completion)
        {
            var fallback = String.IsNullOrEmpty(fallbackBundleFileName) ? null : UIImage.FromBundle(fallbackBundleFileName);

            if (String.IsNullOrEmpty(url))
            {
                this.Image = fallback;
                return;
            }



            var nsUrl = NSUrl.FromString(url);
            if (nsUrl == null)
            {
                this.Image = fallback;
                return;
            }

            ShowLoadingIndicators();
            var placeholder = String.IsNullOrEmpty(placeholderBundleFileName) ? null : UIImage.FromBundle(placeholderBundleFileName);
            this.SetImage(NSUrl.FromString(url), placeholder, delegate (UIImage image, NSError error, SDImageCacheType cache, NSUrl returnedUrl)
            {
                HideLoadingIndicators();
                completion?.Invoke();
            });
        }

        public void ShowLoadingIndicators()
        {
            ActivityIndicator.Hidden = false;
            ActivityIndicator.StartAnimating();
            BringSubviewToFront(ActivityIndicator);
        }
        public void HideLoadingIndicators()
        {
            ActivityIndicator.Hidden = true;
            ActivityIndicator.StopAnimating();
        }
    }
}