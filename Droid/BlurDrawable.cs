using Android.Graphics;
using Android.Renderscripts;
using Android.OS;

public class RSBlurProcessor
{

    private RenderScript rs;

    private static bool IS_BLUR_SUPPORTED = (int)Build.VERSION.SdkInt >= 17;
    private static int MAX_RADIUS = 25;

    public RSBlurProcessor(RenderScript rs)
    {
        this.rs = rs;
    }


    public Bitmap blur(Bitmap bitmap, float radius, int repeat)
    {


        if (!IS_BLUR_SUPPORTED)
        {
            return null;
        }

        if (radius > MAX_RADIUS)
        {
            radius = MAX_RADIUS;
        }

        int width = bitmap.Width;
        int height = bitmap.Height;

        // Create allocation type
        Type bitmapType = new Type.Builder(rs, Element.RGBA_8888(rs))
                .SetX(width)
                .SetY(height)
                 .SetMipmaps(false) // We are using MipmapControl.MIPMAP_NONE
                                  .Create();

        // Create allocation
        Allocation allocation = Allocation.CreateTyped(rs, bitmapType);

        // Create blur script
        ScriptIntrinsicBlur blurScript = ScriptIntrinsicBlur.Create(rs, Element.U8_4(rs));
        blurScript.SetRadius(radius);

        // Copy data to allocation
        allocation.CopyFrom(bitmap);

        // set blur script input
        blurScript.SetInput(allocation);

        // invoke the script to blur
        blurScript.ForEach(allocation);

        // Repeat the blur for extra effect
        for (int i = 0; i < repeat; i++)
        {
            blurScript.ForEach(allocation);
        }

        // copy data back to the bitmap
        allocation.CopyTo(bitmap);

        // release memory
        allocation.Destroy();
        blurScript.Destroy();
        allocation = null;
        blurScript = null;

        return bitmap;
    }
}