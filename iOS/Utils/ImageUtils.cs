using System;
using UIKit;
using Foundation;
using CoreGraphics;
using PCLStorage;
using System.Drawing;

namespace Slink.iOS
{
    public static class ImageUtils
    {
        public static UIImage ScaleImageToWidth(UIImage image, float width)
        {
            width = (float)Math.Round((double)width);

            float oldWidth = (float)image.Size.Width;
            float scaleFactor = width / oldWidth;

            float newHeight = (float)image.Size.Height * scaleFactor;
            float newWidth = oldWidth * scaleFactor;

            return ScaledToSize(image, new CGSize(newWidth, newHeight));
        }

        public static UIImage ScaleImageToFrame(UIImage image, float maxWidth, float maxHeight)
        {
            nfloat oldWidth = image.Size.Width;
            nfloat oldHeight = image.Size.Height;

            nfloat scaleFactor = (oldWidth > oldHeight) ? maxWidth / oldWidth : maxHeight / oldHeight;


            nfloat newHeight = oldHeight * scaleFactor;
            nfloat newWidth = oldWidth * scaleFactor;


            return ScaledToSize(image, new CGSize(newWidth, newHeight));
        }

        public static UIImage ScaledToSize(UIImage image, CGSize newSize)
        {
            CGRect scaledImageRect = new CGRect(0, 0, 0, 0);

            nfloat aspectWidth = newSize.Width / image.Size.Width;
            nfloat aspectHeight = newSize.Height / image.Size.Height;
            nfloat aspectRatio = (nfloat)Math.Min(aspectWidth, aspectHeight);

            scaledImageRect.Size = new CGSize(image.Size.Width * aspectRatio, image.Size.Height * aspectRatio);
            scaledImageRect.X = (newSize.Width - scaledImageRect.Size.Width) / 2;
            scaledImageRect.Y = (newSize.Height - scaledImageRect.Size.Height) / 2;

            UIGraphics.BeginImageContext(newSize);
            image.Draw(scaledImageRect);
            UIImage scaledImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return scaledImage;
        }
        // resize the image to be contained within a maximum width and height, keeping aspect ratio
        public static UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
        {
            var sourceSize = sourceImage.Size;
            var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            if (maxResizeFactor > 1) return sourceImage;
            var width = maxResizeFactor * sourceSize.Width;
            var height = maxResizeFactor * sourceSize.Height;
            UIGraphics.BeginImageContext(new SizeF((float)width, (float)height));
            sourceImage.Draw(new RectangleF(0, 0, (float)width, (float)height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }

        // resize the image (without trying to maintain aspect ratio)
        public static UIImage ResizeImage(UIImage sourceImage, float width, float height)
        {
            UIGraphics.BeginImageContext(new SizeF(width, height));
            sourceImage.Draw(new RectangleF(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }

        // crop the image, without resizing
        private static UIImage CropImage(UIImage sourceImage, int crop_x, int crop_y, int width, int height)
        {
            var imgSize = sourceImage.Size;
            UIGraphics.BeginImageContext(new SizeF(width, height));
            var context = UIGraphics.GetCurrentContext();
            var clippedRect = new RectangleF(0, 0, width, height);
            context.ClipToRect(clippedRect);
            var drawRect = new RectangleF(-crop_x, -crop_y, (float)imgSize.Width, (float)imgSize.Height);
            sourceImage.Draw(drawRect);
            var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return modifiedImage;
        }
        public static UIImage ReduceSizeIfNeededByWidth(UIImage img, float maxWidth = 1000)
        {
            if (img.Size.Width < maxWidth)
            {
                return img;
            }

            return ScaleImageToWidth(img, maxWidth);
        }
        public static UIImage FixOrientation(UIImage image)
        {
            if (image.Orientation == UIImageOrientation.Up)
            {
                return image;
            }


            UIGraphics.BeginImageContextWithOptions(image.Size, false, image.CurrentScale);
            image.Draw(new CGRect(0, 0, image.Size.Width, image.Size.Height));
            UIImage newImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return newImage;
        }

        public static UIImage FromBase64(string base64String)
        {
            if (String.IsNullOrEmpty(base64String))
            {
                return UIImage.FromBundle("MyProfile");
            }
            return UIImage.LoadFromData(NSData.FromArray(Convert.FromBase64String(base64String)));
        }
        public static string ToBase64(UIImage image)
        {
            return image.AsJPEG().GetBase64EncodedData(NSDataBase64EncodingOptions.None).ToString();
        }
        public static UIImage FromByteArray(byte[] bytearray)
        {
            return UIImage.LoadFromData(NSData.FromArray(bytearray));
        }
        public static byte[] ByteArrayFromImage(UIImage image, int quality = 50)
        {
            nfloat myQuality = 1.0f;

            if (quality == 0)
                myQuality = 0.0f;
            else
                myQuality = ((nfloat)quality) / ((nfloat)100);

            using (NSData imageData = image.AsJPEG(myQuality))
            {
                Byte[] myByteArray = new Byte[imageData.Length];
                System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, myByteArray, 0, Convert.ToInt32(imageData.Length));
                return myByteArray;
            }
        }

    }
}

