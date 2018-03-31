using System;
using System.IO;
using System.Net;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.Provider;
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
        public static string GetPath(Context context, Android.Net.Uri directoryUri, Android.Net.Uri fileUri)
        {
            string[] projection = { MediaStore.Images.Media.InterfaceConsts.Data };
            string myPath = string.Empty;

            try
            {
                string myId = DocumentsContract.GetDocumentId(fileUri).Split(':')[1];
                string mySelector = MediaStore.Images.Media.InterfaceConsts.Id + "=?";
                ICursor cursor = context.ContentResolver.Query(directoryUri, projection, mySelector, new string[] { myId }, null);

                if (cursor != null)
                {
                    cursor.MoveToFirst();
                    int dataColumn = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);

                    myPath = cursor.GetString(dataColumn);

                    cursor.Close();
                }
            }
            catch (Exception)
            {
                try
                {
                    ICursor cursor = context.ContentResolver.Query(fileUri, projection, null, null, null);

                    if (cursor != null)
                    {
                        cursor.MoveToFirst();
                        int dataColumn = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);
                        myPath = cursor.GetString(dataColumn);

                        cursor.Close();
                    }
                }
                catch (Exception)
                {
                    myPath = fileUri.Path;
                }
            }

            if (string.IsNullOrEmpty(myPath))
                myPath = fileUri.Path;

            return myPath;
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
        //public static Bitmap GetBitmapFromFile(Java.IO.File Image, int MaxDimension, bool DeletePhoto = true, bool AlwaysRotate = true)
        //{
        //    Bitmap myReturn = null;

        //    if ((Image != null) && (Image.Exists()))
        //    {
        //        ExifInterface myExifInterface = new ExifInterface(Image.Path);
        //        Matrix myMatrix = new Matrix();

        //        int myRotation = myExifInterface.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Undefined);
        //        int finalRotation = 0;

        //        if (myRotation != 0)
        //        {
        //            finalRotation = Atlas.exifToDegrees(myRotation, AlwaysRotate);
        //            myMatrix.PostRotate(finalRotation);
        //        }

        //        using (BitmapFactory.Options myOptions = new BitmapFactory.Options())
        //        {
        //            using (Bitmap myBitmap = BitmapFactory.DecodeFile(Image.Path, myOptions))
        //            {
        //                using (var rotatedBitmap = RotateBitmap(myBitmap, finalRotation))
        //                {
        //                    //already within allowed size
        //                    if (rotatedBitmap.Height <= MaxDimension && rotatedBitmap.Width <= MaxDimension)
        //                    {
        //                        myReturn = rotatedBitmap;
        //                    }
        //                    else
        //                    {
        //                        //height > width and exceeds max dimesnion
        //                        if (rotatedBitmap.Height >= rotatedBitmap.Width && rotatedBitmap.Height > MaxDimension)
        //                        {
        //                            //resize based on height
        //                            var myRatio = ((double)rotatedBitmap.Height / (double)MaxDimension);
        //                            var finalWidth = (int)(rotatedBitmap.Width / myRatio);
        //                            myReturn = ResizeBitmap(rotatedBitmap, finalWidth, MaxDimension);
        //                        }

        //                        //width > height and exceeds max dimesnion
        //                        else if (rotatedBitmap.Width >= rotatedBitmap.Height && rotatedBitmap.Width > MaxDimension)
        //                        {
        //                            //resie based on width
        //                            var myRatio = ((double)rotatedBitmap.Width / (double)MaxDimension);
        //                            var finalHeight = (int)(rotatedBitmap.Height / myRatio);
        //                            myReturn = ResizeBitmap(rotatedBitmap, MaxDimension, finalHeight);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if (DeletePhoto)
        //        Image.Delete();

        //    return myReturn;
        //}

        //public static Bitmap GetBitmapFromMediaStore(Context Context, Android.Net.Uri ContentUri, Android.Net.Uri ImageUri, int MaxDimension, bool AlwaysRotate = true)
        //{
        //    Bitmap myReturn = null;
        //    ExifInterface myExifInterface = new ExifInterface(GetPath(Context, ContentUri, ImageUri));
        //    Matrix myMatrix = new Matrix();

        //    int myRotation = myExifInterface.GetAttributeInt(ExifInterface.TagOrientation, 1);
        //    int myInSampleSize = 1;
        //    int myWidth;
        //    int myHeight;

        //    if (myRotation != 0f)
        //        myMatrix.PreRotate(Atlas.exifToDegrees(myRotation, AlwaysRotate));

        //    using (BitmapFactory.Options myOptions = new BitmapFactory.Options())
        //    {
        //        int myImageWidth = int.MinValue;
        //        int myImageHeight = int.MinValue;
        //        double myRatio;

        //        myOptions.InJustDecodeBounds = true;

        //        using (AssetFileDescriptor myFileDescriptor = Context.ContentResolver.OpenAssetFileDescriptor(ImageUri, "r"))
        //        {
        //            using (Bitmap myBitmap = BitmapFactory.DecodeFileDescriptor(myFileDescriptor.FileDescriptor, null, myOptions))
        //            {
        //                myImageWidth = myOptions.OutWidth;
        //                myImageHeight = myOptions.OutHeight;
        //            }
        //        }

        //        if ((myImageWidth != int.MinValue) && (myImageHeight != int.MinValue))
        //        {
        //            // Resize based on image dimensions
        //            if (myImageWidth == myImageHeight)
        //            {
        //                if ((myImageWidth < MaxDimension) && (myImageHeight < MaxDimension))
        //                {
        //                    myWidth = myImageWidth;
        //                    myHeight = myImageHeight;
        //                }
        //                else
        //                {
        //                    myWidth = MaxDimension;
        //                    myHeight = MaxDimension;
        //                }
        //            }
        //            else if (myImageHeight > myImageWidth)
        //            {
        //                // Prevent image expansion
        //                if (myImageHeight < MaxDimension)
        //                    MaxDimension = myImageHeight;

        //                myRatio = ((double)myImageHeight / (double)MaxDimension);

        //                myWidth = (int)(myImageWidth / myRatio);
        //                myHeight = MaxDimension;
        //            }
        //            else
        //            {
        //                if ((myImageWidth < MaxDimension) && (myImageHeight < MaxDimension))
        //                {
        //                    myWidth = myImageWidth;
        //                    myHeight = myImageHeight;
        //                }
        //                else
        //                {
        //                    // Prevent image expansion
        //                    if (myImageWidth < MaxDimension)
        //                        MaxDimension = myImageWidth;

        //                    myRatio = ((double)myImageWidth / (double)MaxDimension);

        //                    myWidth = MaxDimension;
        //                    myHeight = (int)(myImageHeight / myRatio);
        //                }
        //            }

        //            if ((myRotation == 1) || (myRotation == 3) || (myRotation == 6))
        //            {
        //                bool mySwapSizes = true;

        //                if ((!AlwaysRotate) && (myRotation == 1))
        //                    mySwapSizes = false;

        //                if (mySwapSizes)
        //                {
        //                    int myTempSize = myImageWidth;

        //                    myImageWidth = myImageHeight;
        //                    myImageHeight = myTempSize;

        //                    myTempSize = myWidth;
        //                    myWidth = myHeight;
        //                    myHeight = myTempSize;
        //                }
        //            }

        //            if ((myImageHeight > myHeight) || (myImageWidth > myWidth))
        //            {
        //                int myImageHalfHeight = myImageHeight / 2;
        //                int myImageHalfWidth = myImageWidth / 2;

        //                while (((myImageHalfHeight / myInSampleSize) > myHeight) && ((myImageHalfWidth / myInSampleSize) > myWidth))
        //                    myInSampleSize *= 2;

        //                myOptions.InJustDecodeBounds = false;
        //                myOptions.InSampleSize = myInSampleSize;

        //                using (AssetFileDescriptor myFileDescriptor = Context.ContentResolver.OpenAssetFileDescriptor(ImageUri, "r"))
        //                {
        //                    Bitmap myBitmap = BitmapFactory.DecodeFileDescriptor(myFileDescriptor.FileDescriptor, null, myOptions);

        //                    myBitmap = Bitmap.CreateBitmap(myBitmap, 0, 0, myBitmap.Width, myBitmap.Height, myMatrix, true);
        //                    myReturn = Bitmap.CreateScaledBitmap(myBitmap, myWidth, myHeight, false);

        //                    //myBitmap.Recycle();
        //                    //myBitmap.Dispose();
        //                }
        //            }
        //            else
        //            {
        //                using (AssetFileDescriptor myFileDescriptor = Context.ContentResolver.OpenAssetFileDescriptor(ImageUri, "r"))
        //                {
        //                    Bitmap myBitmap = BitmapFactory.DecodeFileDescriptor(myFileDescriptor.FileDescriptor, null, null);

        //                    myReturn = Bitmap.CreateBitmap(myBitmap, 0, 0, myBitmap.Width, myBitmap.Height, myMatrix, true);

        //                    //myBitmap.Recycle();
        //                    //myBitmap.Dispose();
        //                }
        //            }
        //        }
        //    }

        //    return myReturn;
        //}
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