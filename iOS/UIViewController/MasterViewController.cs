using System;
using UIKit;

namespace Slink.iOS
{
    public class MasterViewController : ActivatorContainerViewController
    {
        DrawerViewController DrawerViewController;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Hamburger"), UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                if (DrawerViewController == null)
                {
                    DrawerViewController = new DrawerViewController();
                    DrawerViewController.TransitioningDelegate = new ShowDrawerTransitionDelegate(DrawerDirection.Left, 270);
                    DrawerViewController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                }
                PresentViewController(DrawerViewController, true, null);
            });

            ApplicationExtensions.ActivatePage(typeof(MyCardsViewController), false, false);
        }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (IsSufficentPermissionGranted())
                ServiceRunner.SharedInstance.StartService<GeolocatorService>();

        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                DrawerViewController?.Dispose();
                DrawerViewController = null;
            }
        }
    }
}
