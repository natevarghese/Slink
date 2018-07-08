using System;
using CoreLocation;
using UIKit;

namespace Slink.iOS
{
    public class LocationManager : CLLocationManager
    {
        protected CLLocationManager locMgr = new CLLocationManager();

        public LocationManager()
        {
            this.locMgr.PausesLocationUpdatesAutomatically = false;

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
                locMgr.AllowsBackgroundLocationUpdates = false;
        }

        public CLLocationManager LocMgr
        {
            get { return this.locMgr; }
        }


        public static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }
    }


    public class LocationUpdatedEventArgs : EventArgs
    {
        CLLocation location;

        public LocationUpdatedEventArgs(CLLocation location)
        {
            this.location = location;
        }

        public CLLocation Location
        {
            get { return location; }
        }
    }
}

