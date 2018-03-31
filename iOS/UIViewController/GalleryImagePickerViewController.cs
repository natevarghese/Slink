using System;

using UIKit;

namespace Slink.iOS
{
    public partial class GalleryImagePickerViewController : UIViewController
    {
        public GalleryImagePickerViewController() : base("GalleryImagePickerViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }



    public class GalleryImagePicker : UIImagePickerController
    {
        public GalleryImagePicker()
        {
            SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.SavedPhotosAlbum);
            AllowsEditing = true;
            NavigationBar.BarTintColor = UIColor.Black;
            NavigationBar.TintColor = UIColor.White;
            NavigationBar.TitleTextAttributes = new UIStringAttributes()
            {
                ForegroundColor = UIColor.White
            };
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }
    }
}

