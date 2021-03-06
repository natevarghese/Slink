﻿using System;
using System.Drawing;
using Plugin.CurrentActivity;

namespace Slink.Droid
{

    public class BroadCastNotificaion : IBroadcastNotificaion
    {
        public void SendNotificaion(string key)
        {
            var intent = new Android.Content.Intent(key);

            CrossCurrentActivity.Current.Activity?.SendBroadcast(intent);
        }

    }

    public class Database : IDatabase
    {
        public string GetDatabasePath()
        {
            throw new NotImplementedException();
            return null;
            //return NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
        }
    }

    public class Facebook : IFacebook
    {
        public void Logout()
        {
            throw new NotImplementedException();
            //new LoginManager().LogOut();
        }
    }

    public class Navigation : INavigation
    {
        public void GoToLists()
        {
            throw new NotImplementedException();
            //var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            //appDelegate.Window.RootViewController = UIStoryboard.FromName("Lists", null).InstantiateInitialViewController();
        }

        public void GoToMain()
        {
            throw new NotImplementedException();
            //var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            //appDelegate.Window.RootViewController = UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
        }
    }



    public class S3Service : IS3Service
    {

        public byte[] ResizeImage(byte[] imageData, float width, float height)
        {
            return null;
            //UIImage originalImage = ImageUtils.FromByteArray(imageData);
            //float maxWidth = 200;
            //float maxHeight = 200;
            //float calculatedWidth = width;
            //float calculatedHeight = height;

            //var sourceSize = originalImage.Size;
            //var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            //if (maxResizeFactor > 1)
            //    return originalImage.AsJPEG().ToArray();
            //calculatedWidth = (float)maxResizeFactor * (float)sourceSize.Width;
            //calculatedHeight = (float)maxResizeFactor * (float)sourceSize.Height;

            //UIGraphics.BeginImageContext(new SizeF(calculatedWidth, calculatedHeight));
            //originalImage.Draw(new RectangleF(0, 0, calculatedWidth, calculatedHeight));
            //var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            //UIGraphics.EndImageContext();

            //string AvatarImageName = "avatar.jpg";
            //string PersonalFolderPath = "";
            //string avatarFilename = System.IO.Path.Combine(PersonalFolderPath, AvatarImageName);
            //resultImage.AsJPEG().Save(avatarFilename, true);

            //return resultImage.AsJPEG().ToArray();
        }
    }
}
