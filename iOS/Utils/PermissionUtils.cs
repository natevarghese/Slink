using System;
using Photos;
using UIKit;
using Foundation;

namespace Slink.iOS
{
    public static class PermissionUtils
    {
        public static void AskForLocationPermissionIfNecessary()
        {

        }
        public static bool CheckForLocationPermissionGranted(UIViewController vc)
        {
            if (vc == null)
                return false;

            if (CoreLocation.CLLocationManager.Status != CoreLocation.CLAuthorizationStatus.AuthorizedAlways && CoreLocation.CLLocationManager.Status != CoreLocation.CLAuthorizationStatus.AuthorizedWhenInUse)
            {
                UIAlertController alertController = UIAlertController.Create(Strings.Basic.error, Strings.Permissions.location_disabled, UIAlertControllerStyle.Alert);
                alertController.AddAction(UIAlertAction.Create(Strings.Basic.settings, UIAlertActionStyle.Default, delegate
                {
                    UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(UIApplication.OpenSettingsUrlString));
                }));
                alertController.AddAction(UIAlertAction.Create(Strings.Basic.ok, UIAlertActionStyle.Cancel, delegate
                {
                    alertController.DismissViewController(true, null);
                }));
                vc.PresentViewController(alertController, true, null);
                return false;
            }
            return true;
        }
        public static bool CheckForPhotoPermissionGranted(UIViewController vc)
        {
            if (vc == null)
                return false;

            if (PHPhotoLibrary.AuthorizationStatus == PHAuthorizationStatus.NotDetermined)
            {
                PHPhotoLibrary.RequestAuthorization((PHAuthorizationStatus obj) =>
                    {
                        if (obj != PHAuthorizationStatus.Authorized)
                        {
                            CheckForPhotoPermissionGranted(vc);
                            return;
                        }
                    });
            }
            else if (PHPhotoLibrary.AuthorizationStatus != PHAuthorizationStatus.Authorized)
            {
                UIAlertController alertController = UIAlertController.Create(Strings.Basic.error, Strings.Permissions.photos_disabled, UIAlertControllerStyle.Alert);
                alertController.AddAction(UIAlertAction.Create(Strings.Basic.settings, UIAlertActionStyle.Default, delegate
                {
                    UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(UIApplication.OpenSettingsUrlString));
                }));
                alertController.AddAction(UIAlertAction.Create(Strings.Basic.ok, UIAlertActionStyle.Cancel, delegate
                {
                    alertController.DismissViewController(true, null);
                }));
                vc.PresentViewController(alertController, true, null);
                return false;
            }

            return true;

        }
    }
}
