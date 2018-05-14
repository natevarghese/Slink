using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using PCLStorage;

namespace Slink.Droid
{
    public static class ImageUtils
    {

        public static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }

        public static int GetResoueceIdFromFileName(Context context, string fileName, string packageName)
        {
            try
            {
                return context.Resources.GetIdentifier(fileName, "drawable", packageName);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public static string ImageToBase64(Android.Graphics.Bitmap image, int quality = 90)
        {
            var stream = new System.IO.MemoryStream();
            //using ()
            //{
            image.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, quality, stream);

            //float mb = (stream.ToArray().Length / 1024f) / 1024f;

            return Convert.ToBase64String(stream.ToArray());
            //}
        }

        public static byte[] ImagetoByteArray(Android.Graphics.Bitmap bitmap, int quality = 90)
        {
            byte[] bitmapData;
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, quality, stream);
                bitmapData = stream.ToArray();
            }

            return bitmapData;
        }

        public static Android.Graphics.Bitmap ImageFromBase64(string base64)
        {
            if (!string.IsNullOrEmpty(base64))
            {
                byte[] encodeByte = null;

                try
                {
                    encodeByte = Android.Util.Base64.Decode(base64, Android.Util.Base64Flags.Default);
                }
                catch (Exception ex)
                {

                }

                return ImageFromByteArray(encodeByte);
            }
            else
                return null;
        }

        public static Android.Graphics.Bitmap ImageFromByteArray(byte[] imagebytearray)
        {
            if (imagebytearray == null)
                return null;

            return Android.Graphics.BitmapFactory.DecodeByteArray(imagebytearray, 0, imagebytearray.Length);
        }

        public static Bitmap GetCroppedBitmap(Bitmap bmp, int radius, int borderWidth = 2, string borderColor = "#AAAAAA")
        {
            if (bmp == null) return Bitmap.CreateBitmap(0, 0, Bitmap.Config.Argb8888);

            Bitmap sbmp;

            radius = (radius == 0) ? bmp.Width / 2 : radius;

            if (bmp.Width != radius || bmp.Height != radius)
                sbmp = Bitmap.CreateScaledBitmap(bmp, radius, radius, false);
            else
                sbmp = bmp;

            Bitmap output = Bitmap.CreateBitmap(sbmp.Width, sbmp.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(output);

            Paint paint = new Paint();
            Rect rect = new Rect(0, 0, sbmp.Width, sbmp.Height);

            paint.AntiAlias = true;
            paint.FilterBitmap = true;
            paint.Dither = true;
            canvas.DrawARGB(0, 0, 0, 0);
            canvas.DrawCircle((sbmp.Width - borderWidth) / 2, (sbmp.Height - borderWidth) / 2, (sbmp.Width / 2) - borderWidth, paint);
            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            canvas.DrawBitmap(sbmp, rect, rect, paint);

            paint = new Paint();
            paint.Color = Android.Graphics.Color.ParseColor(borderColor);
            paint.SetStyle(Paint.Style.Stroke);
            paint.AntiAlias = true;
            paint.StrokeWidth = borderWidth;
            canvas.DrawCircle((sbmp.Width - borderWidth) / 2, (sbmp.Height - borderWidth) / 2, (sbmp.Width / 2) - borderWidth, paint);

            sbmp.Dispose();

            canvas.Dispose();
            paint.Dispose();
            rect.Dispose();

            return output;
        }

        public static Bitmap AddBlackBorder(Bitmap bmp, int borderSize)
        {
            Bitmap bmpWithBorder = Bitmap.CreateBitmap(bmp.Width + borderSize * 2, bmp.Height + borderSize * 2, bmp.GetConfig());
            Canvas canvas = new Canvas(bmpWithBorder);
            canvas.DrawColor(Android.Graphics.Color.Black);
            canvas.DrawBitmap(bmp, borderSize, borderSize, null);

            return bmpWithBorder;
        }
        public static Bitmap CreateRepeatImageBitmap(Bitmap image, int width, int height)
        {
            if (width <= 0) width = 1;
            if (height <= 0) height = 1;

            Bitmap cs = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(cs);
            for (int i = 0; i < width; i = i + image.Width)
            {
                for (int j = 0; j < height; j = j + image.Height)
                {
                    canvas.DrawBitmap(image, i, j, null);
                }
            }

            return cs;
        }

        public static Bitmap BetRoundedCornerWithBorderBitmap(Bitmap bitmap, int cornerDips, int borderDips, Context context, int borderWidth)
        {
            Bitmap output = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height,
                                                Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(output);

            int cornerSizePx = (int)Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Dip, (float)cornerDips, context.Resources.DisplayMetrics);
            Paint paint = new Paint();
            Rect rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
            RectF rectF = new RectF(rect);

            // prepare canvas for transfer
            paint.AntiAlias = true;
            paint.Color = Android.Graphics.Color.White;
            paint.SetStyle(Paint.Style.Fill);
            canvas.DrawARGB(0, 0, 0, 0);
            canvas.DrawRoundRect(rectF, cornerSizePx, cornerSizePx, paint);

            // draw bitmap
            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            canvas.DrawBitmap(bitmap, rect, rect, paint);

            // draw border
            paint.Color = Android.Graphics.Color.Black;
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = (float)borderWidth;
            canvas.DrawRoundRect(rectF, cornerSizePx, cornerSizePx, paint);

            return output;
        }


        public static Bitmap CreateThumbnail(Bitmap source, int width, int height)
        {
            int myWidth, myHeight;

            if (source.Height == source.Width)
            {
                myWidth = width;
                myHeight = height;
            }
            else if (source.Height > source.Width)
            {
                double myRatio = ((double)source.Height / (double)height);

                myHeight = height;
                myWidth = (int)(source.Width / myRatio);
            }
            else
            {
                double myRatio = ((double)source.Width / (double)width);

                myHeight = (int)(source.Height / myRatio);
                myWidth = width;
            }

            return Bitmap.CreateScaledBitmap(source, myWidth, myHeight, false);
        }
        public static Drawable BitmapIntoDrawable(Bitmap bitmap)
        {
            BitmapDrawable tile = new BitmapDrawable(bitmap);
            return tile;
        }
        public static Bitmap GetBitmapfromResouceFileName(Context context, string filename)
        {
            var id = GetResoueceIdFromFileName(context, filename, context.PackageName);
            if (id != -1)
                return BitmapFactory.DecodeResource(context.Resources, id);

            return null;
        }
        async public static Task<Bitmap> GetImageAtPath(string path)
        {
            if (String.IsNullOrEmpty(path)) return null;

            var file = await FileSystem.Current.GetFileFromPathAsync(path);
            if (file == null) return null;

            using (var stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
            {
                if (stream == null) return null;

                return BitmapFactory.DecodeStream(stream);
            }
        }
        public static Bitmap RotateBitmap(Bitmap source, float angle)
        {
            Matrix matrix = new Matrix();
            matrix.PostRotate(angle);
            return Bitmap.CreateBitmap(source, 0, 0, source.Width, source.Height, matrix, true);
        }
        public static Bitmap ResizeBitmap(Bitmap image, int maxWidth, int maxHeight)
        {
            if (maxHeight > 0 && maxWidth > 0)
            {
                int width = image.Width;
                int height = image.Height;
                float ratioBitmap = (float)width / (float)height;
                float ratioMax = (float)maxWidth / (float)maxHeight;

                int finalWidth = maxWidth;
                int finalHeight = maxHeight;
                if (ratioMax > ratioBitmap)
                {
                    finalWidth = (int)((float)maxHeight * ratioBitmap);
                }
                else
                {
                    finalHeight = (int)((float)maxWidth / ratioBitmap);
                }
                image = Bitmap.CreateScaledBitmap(image, finalWidth, finalHeight, true);
                return image;
            }
            else
            {
                return image;
            }
        }
        public static long MegabytesAvailable()
        {
            long megAvailable = Android.OS.Environment.DataDirectory.UsableSpace / (1024 * 1024);
            Console.WriteLine("MegabytesAvailable :" + megAvailable);

            return megAvailable;
        }
        public static Bitmap GetHexagonShape(Bitmap scaleBitmapImage)
        {
            if (scaleBitmapImage == null) return null;

            int targetWidth = 200;
            int targetHeight = 200;
            int drawnWidth = 140;

            Bitmap targetBitmap = Bitmap.CreateBitmap(targetWidth, targetHeight, Bitmap.Config.Argb8888);

            Canvas canvas = new Canvas(targetBitmap);

            Android.Graphics.Path path = new Android.Graphics.Path();

            //start top left
            path.MoveTo(0, 0);

            //move to middle top
            path.LineTo(drawnWidth, 0);

            //move to bottom right
            path.LineTo(targetWidth, targetHeight - 0);

            //move to bottom middle
            path.LineTo(targetWidth - drawnWidth, targetHeight - 0);

            //middle top left
            path.MoveTo(0, 0);


            canvas.ClipPath(path);
            Bitmap sourceBitmap = scaleBitmapImage;
            canvas.DrawBitmap(sourceBitmap, new Rect(0, 0, sourceBitmap.Width, sourceBitmap.Height), new Rect(0, 0, targetWidth, targetHeight), null);
            return targetBitmap;
        }
    }
}