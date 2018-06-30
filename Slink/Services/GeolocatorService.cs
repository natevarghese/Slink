using System;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.Geolocator;
using System.Threading.Tasks;
using Plugin.Geolocator.Abstractions;

namespace Slink
{
    public class GeolocatorService : BaseQueue
    {
        override async public void Start()
        {
            if (Running) return;

            if (!CrossGeolocator.IsSupported) return;
            if (!CrossGeolocator.Current.IsGeolocationEnabled) return;
            if (!CrossGeolocator.Current.IsGeolocationAvailable) return;
            if (CrossGeolocator.Current.IsListening) return;
            if (ShouldStop) return;

            var permissionGrantedAlways = await HasPermission(Permission.LocationAlways);
            var permissionGrantedWhileUsing = await HasPermission(Permission.LocationWhenInUse);
            if (!permissionGrantedAlways && !permissionGrantedWhileUsing) return;

            try
            {
                await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(3), 1, true, new ListenerSettings()
                {
                    ActivityType = ActivityType.Fitness,
                    AllowBackgroundUpdates = false,
                    DeferLocationUpdates = false,
                    DeferralDistanceMeters = 1,
                    DeferralTime = TimeSpan.FromSeconds(3),
                    ListenForSignificantChanges = false,
                    PauseLocationUpdatesAutomatically = false
                });
                System.Diagnostics.Debug.WriteLine("GeolocatorService Started");
            }
            catch (UnauthorizedAccessException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            CrossGeolocator.Current.PositionChanged -= Current_PositionChanged;
            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
        }

        public async Task<bool> HasPermission(Permission permission)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
            return status == PermissionStatus.Granted;
        }

        public async Task AskPermissionIfNecessary(Permission[] permissions)
        {
            await CrossPermissions.Current.RequestPermissionsAsync(permissions);
        }
        //stop the queue, tell everyone listening.
        public override void Reset()
        {
            CrossGeolocator.Current.PositionChanged -= Current_PositionChanged;
            CrossGeolocator.Current.StopListeningAsync();
            System.Diagnostics.Debug.WriteLine("GeolocatorService Ended");

            base.Reset();
        }

        void Current_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            LocationReceived(e.Position);



            //String aText = null;
            //var test = e.Position;
            //aText = "Full: Lat: " + test.Latitude.ToString() + " Long: " + test.Longitude.ToString();
            //aText += "\n" + $"Time: {test.Timestamp.ToString()}";
            //aText += "\n" + $"Heading: {test.Heading.ToString()}";
            //aText += "\n" + $"Speed: {test.Speed.ToString()}";
            //aText += "\n" + $"Accuracy: {test.Accuracy.ToString()}";
            //aText += "\n" + $"Altitude: {test.Altitude.ToString()}";
            //aText += "\n" + $"AltitudeAccuracy: {test.AltitudeAccuracy.ToString()}";

            //Console.WriteLine(aText);
        }

        async void LocationReceived(Position position)
        {
            var location = new UserLocation();
            location.Latitude = position.Latitude;//34.0647985019681
            location.Longitude = position.Longitude;//-118.312238891951
            location.Accuracy = position.Accuracy;

            RealmServices.SaveUserLocation(location);

            try
            {
                await WebServices.UserController.UpdateUser(location.Latitude, location.Longitude);
            }
            catch (Exception) { }
        }

        //Helper function to send a broadcast notification
        void SendBroadcast(string message)
        {
            IBroadcastNotificaion iBroadcast = ServiceLocator.Instance.Resolve<IBroadcastNotificaion>();
            if (iBroadcast == null) return;

            iBroadcast.SendNotificaion(message);
        }
    }
}