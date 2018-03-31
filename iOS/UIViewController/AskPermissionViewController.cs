using System;
using CoreLocation;
using UIKit;

namespace Slink.iOS
{
    public class AskPermissionViewController : UIViewController
    {
        public bool LocationEnabled = false;
        public LocationManager LocationManager;

        public AskPermissionViewController() : base() { }
        public AskPermissionViewController(IntPtr handle) : base(handle) { }
        public AskPermissionViewController(string xibName) : base(xibName, null) { }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            if (LocationManager != null)
                LocationManager.AuthorizationChanged -= LocationManager_AuthorizationChanged;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                LocationManager?.Dispose();
                LocationManager = null;
            }
        }
        public void AskLocationPermissionIfNecessary()
        {
            if (!IsSufficentPermissionGranted())
            {
                RequestPermissions();
            }
        }

        public bool IsSufficentPermissionGranted()
        {
            var granted = CLLocationManager.LocationServicesEnabled &&
                                           CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways ||
                                           CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse;
            return granted;
        }
        public bool ArePermissionUnknown()
        {
            return CLLocationManager.Status == CLAuthorizationStatus.NotDetermined;
        }

        public void RequestPermissions()
        {
            LocationManager?.RequestAlwaysAuthorization();
        }

        async public void StartLocationManager()
        {
            if (!ServiceRunner.SharedInstance.ContainsService<GeolocatorService>())
                ServiceRunner.SharedInstance.AddService<GeolocatorService>();

            var service = ServiceRunner.SharedInstance.FetchService<GeolocatorService>();
            await service.AskPermissionIfNecessary(new Plugin.Permissions.Abstractions.Permission[] { Plugin.Permissions.Abstractions.Permission.LocationWhenInUse });
            service.Start();

            if (LocationManager == null)
                LocationManager = new LocationManager();
            LocationManager.AuthorizationChanged -= LocationManager_AuthorizationChanged;
            LocationManager.AuthorizationChanged += LocationManager_AuthorizationChanged;
        }

        public virtual void LocationManager_AuthorizationChanged(object sender, CLAuthorizationChangedEventArgs e) { }
    }
}
