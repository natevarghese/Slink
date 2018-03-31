using System;
using System.Drawing;
using Facebook.LoginKit;
using Foundation;
using UIKit;

namespace Slink.iOS
{

    public class BroadCastNotificaion : IBroadcastNotificaion
    {
        public void SendNotificaion(string key)
        {
            NSNotificationCenter.DefaultCenter.PostNotificationName(key, null);
        }
    }

    public class Database : IDatabase
    {
        public string GetDatabasePath()
        {
            return NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
        }
    }

    public class Facebook : IFacebook
    {
        public void Logout()
        {
            new LoginManager().LogOut();
        }
    }

    public class Navigation : INavigation
    {
        public void GoToLists()
        {
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            appDelegate.Window.RootViewController = UIStoryboard.FromName("Lists", null).InstantiateInitialViewController();
        }

        public void GoToMain()
        {
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            appDelegate.Window.RootViewController = UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
        }
    }



    public class S3Service : IS3Service
    {

        public byte[] ResizeImage(byte[] imageData, float width, float height)
        {
            UIImage originalImage = ImageUtils.FromByteArray(imageData);
            float maxWidth = 200;
            float maxHeight = 200;
            float calculatedWidth = width;
            float calculatedHeight = height;

            var sourceSize = originalImage.Size;
            var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            if (maxResizeFactor > 1)
                return originalImage.AsJPEG().ToArray();
            calculatedWidth = (float)maxResizeFactor * (float)sourceSize.Width;
            calculatedHeight = (float)maxResizeFactor * (float)sourceSize.Height;

            UIGraphics.BeginImageContext(new SizeF(calculatedWidth, calculatedHeight));
            originalImage.Draw(new RectangleF(0, 0, calculatedWidth, calculatedHeight));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            string AvatarImageName = "avatar.jpg";
            string PersonalFolderPath = "";
            string avatarFilename = System.IO.Path.Combine(PersonalFolderPath, AvatarImageName);
            resultImage.AsJPEG().Save(avatarFilename, true);

            return resultImage.AsJPEG().ToArray();
        }
    }
}
