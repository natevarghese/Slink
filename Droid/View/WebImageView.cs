using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Work;
using FFImageLoading.Transformations;
using Android.App;

namespace Slink.Droid
{
    [Register("com.nvcomputers.slink.WebImageView")]
    public class WebImageView : FFImageLoading.Views.ImageViewAsync
    {
        public static CircleTransformation DefaultCircleTransformation = new CircleTransformation(0, "#ffffff");

        ProgressBar ProgressBar;

        public WebImageView(Context context) : base(context) { }
        public WebImageView(Context context, IAttributeSet attributeSet) : base(context, attributeSet) { }
        public WebImageView(Context context, IAttributeSet attributeSet, int defStyle) : base(context, attributeSet, defStyle) { }
        public WebImageView(IntPtr ptr, JniHandleOwnership handle) : base(ptr, handle) { }

        public void SetImage(string url, int placeholderResource, int fallbackResource, string cacheKey, ITransformation transformation)
        {
            if (String.IsNullOrEmpty(url))
            {
                if (fallbackResource > 0)
                    SetImageResource(fallbackResource);
                return;
            }

            ShowLoadingIndicators();

            if (placeholderResource > 0)
                SetImageResource(placeholderResource);


            ImageService.Instance.LoadUrl(url).CacheKey(cacheKey).Transform(transformation)
                .Success((arg1, arg2) =>
                {

                })
                .Error((obj) =>
                {
                    ((Activity)Context).RunOnUiThread(() =>
                    {
                        if (fallbackResource > 0)
                            SetImageResource(fallbackResource);
                    });
                }).Finish((obj) =>
                {
                    HideLoadingIndicators();
                }).Into(this);
        }
        public void ShowLoadingIndicators()
        {
            ProgressBar = ProgressBar ?? new ProgressBar(Context);
        }
        public void HideLoadingIndicators()
        {

        }
        protected override void JavaFinalize()
        {
            SetImageDrawable(null);
            SetImageBitmap(null);

            //todo needed?
            //ImageService.Instance.InvalidateCacheEntryAsync(this.DataLocationUri, FFImageLoading.Cache.CacheType.Memory);

            base.JavaFinalize();
        }
    }
}