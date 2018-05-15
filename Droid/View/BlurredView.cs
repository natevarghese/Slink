using System;
using Android.Content;
using Android.Views;
using BlurBehindSdk;

namespace Slink.Droid
{
    public class BlurredView : View
    {
        public BlurredView(IntPtr ptr, Android.Runtime.JniHandleOwnership transfer) : base(ptr, transfer) { }
        public BlurredView(Context context) : base(context) { }

        public View mBlurredView;

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            base.OnDraw(canvas);

            // Use the blurred view’s draw() method to draw on a private canvas.
            //mBlurredView.Draw(mBlurringCanvas);

            //// Blur the bitmap backing the private canvas into mBlurredBitmap
            //blur();

            //// Draw mBlurredBitmap with transformations on the blurring view’s main canvas.
            //canvas.save();
            //canvas.translate(mBlurredView.getX() - getX(), mBlurredView.getY() - getY());
            //canvas.scale(DOWNSAMPLE_FACTOR, DOWNSAMPLE_FACTOR);
            //canvas.drawBitmap(mBlurredBitmap, 0, 0, null);
            //canvas.restore();
        }
    }
}
