using System;
using SDWebImage;
using UIKit;
using Foundation;
using System.Linq;

namespace Slink.iOS
{
    public partial class WebImageButton : UIButton
    {
        UIView DimmedView = new UIView();
        public UIActivityIndicatorViewStyle ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.White;

        public WebImageButton(IntPtr handle) : base(handle)
        {
            ClipsToBounds = true;

            //content mode
            VerticalAlignment = UIControlContentVerticalAlignment.Fill;
            HorizontalAlignment = UIControlContentHorizontalAlignment.Fill;
        }


        public void SetImage(string localURL, string fallbackBundleFileName)
        {
            var fallback = String.IsNullOrEmpty(fallbackBundleFileName) ? null : UIImage.FromBundle(fallbackBundleFileName);

            if (String.IsNullOrEmpty(localURL))
            {
                this.SetImage(fallback, new UIControlState());
                return;
            }

            var image = UIImage.FromFile(localURL);
            if (image != null)
            {
                SetImage(image, new UIControlState());
            }
            else
            {
                this.SetImage(fallback, new UIControlState());
            }

            HideLoadingIndicators();
        }


        public void SetImageWithCustomCache(string url, string placeholderBundleFileName, string fallbackBundleFileName, string cacheKey)
        {
            var fallback = String.IsNullOrEmpty(fallbackBundleFileName) ? null : UIImage.FromBundle(fallbackBundleFileName);
            var placeholder = String.IsNullOrEmpty(placeholderBundleFileName) ? null : UIImage.FromBundle(placeholderBundleFileName);

            var cachedImageMemory = SDWebImageManager.SharedManager.ImageCache.ImageFromMemoryCache(cacheKey);
            if (cachedImageMemory != null) { SetImage(cachedImageMemory, new UIControlState()); return; }

            var cachedImageDisk = SDWebImageManager.SharedManager.ImageCache.ImageFromDiskCache(cacheKey);
            if (cachedImageDisk != null) { SetImage(cachedImageDisk, new UIControlState()); return; }

            this.SetImage(NSUrl.FromString(url), new UIControlState(), placeholder, SDWebImageOptions.RetryFailed, delegate (UIImage image, NSError error, SDImageCacheType cacheType, NSUrl imageUrl)
            {
                HideLoadingIndicators();

                if (image == null || error != null)
                    this.SetImage(fallback, new UIControlState());
                else
                    SDImageCache.SharedImageCache.StoreImage(image, cacheKey, true, null);
            });
        }
        public void SetImage(string url, string placeholderBundleFileName, string fallbackBundleFileName)
        {

            var fallback = String.IsNullOrEmpty(fallbackBundleFileName) ? null : UIImage.FromBundle(fallbackBundleFileName);

            if (String.IsNullOrEmpty(url))
            {
                HideLoadingIndicators();
                this.SetImage(fallback, new UIControlState());
                return;
            }

            ShowLoadingIndicators();

            var placeholder = String.IsNullOrEmpty(placeholderBundleFileName) ? null : UIImage.FromBundle(placeholderBundleFileName);
            this.SetImage(NSUrl.FromString(url), new UIControlState(), placeholder, SDWebImageOptions.RetryFailed, delegate (UIImage image, NSError error, SDImageCacheType cacheType, NSUrl imageUrl)
            {
                if (image == null || error != null)
                    this.SetImage(fallback, new UIControlState());

                HideLoadingIndicators();
            });
        }

        void RemoveActivityIndicators()
        {
            //existing acitvity indicators
            foreach (var ai in Subviews.Where(c => c.GetType() == typeof(UIActivityIndicatorView)))
                ai.RemoveFromSuperview();
        }
        public void ShowLoadingIndicators()
        {
            RemoveActivityIndicators();

            var activityIndicator = new UIActivityIndicatorView(ActivityIndicatorViewStyle);
            activityIndicator.UserInteractionEnabled = false;
            activityIndicator.Hidden = false;
            activityIndicator.StartAnimating();
            AddSubview(activityIndicator);
        }
        public void ShowDimmedView()
        {
            if (!Subviews.Contains(DimmedView))
            {
                DimmedView.UserInteractionEnabled = false;
                DimmedView.Alpha = .45f;
                DimmedView.BackgroundColor = UIColor.Black;
                AddSubview(DimmedView);
            }
            DimmedView.Frame = Frame;
            DimmedView.Hidden = false;
            BringSubviewToFront(DimmedView);
        }
        public void HideLoadingIndicators()
        {
            RemoveActivityIndicators();

            DimmedView.Hidden = true;
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            DimmedView.Frame = Bounds;

            foreach (var ai in Subviews.Where(c => c.GetType() == typeof(UIActivityIndicatorView)))
                ai.Frame = Bounds;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                DimmedView?.RemoveFromSuperview();
                DimmedView?.Dispose();
                DimmedView = null;
            }
        }
    }
}

