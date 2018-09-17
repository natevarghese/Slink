using System; using CoreAnimation; using CoreGraphics; using CoreLocation;
using Foundation; using UIKit; using System.Linq; using System.Threading.Tasks; using Plugin.Permissions.Abstractions;
using System.Timers;

namespace Slink.iOS {     public partial class CardSharingStatusViewController : BaseHeaderFooterViewController     {         public Card SelectedCard;         public bool Sharing;          string SessionUUID;         bool ButtonLocked;          Timer Timer = new Timer();         CAShapeLayer shape;         NSObject ResignActiveNotification;          //Used for Onboarding         public bool DisplayPurposeOnly;         public Action FirstTapInitiatedAction;          public CardSharingStatusViewController() : base("CardSharingStatusViewController", null) { }          public override void ViewDidLoad()
        {             NetworkListenerEnabled = false; 
            base.ViewDidLoad();

            //adds a stationary circle around phone
            CAShapeLayer first = new CAShapeLayer();
            first.Bounds = new CGRect(0, 0, PhoneButton.Frame.Width, PhoneButton.Frame.Height);
            first.Position = new CGPoint(PhoneButton.Frame.Width / 2, PhoneButton.Frame.Height / 2);
            first.Path = UIBezierPath.FromOval(PhoneButton.Bounds).CGPath;
            first.StrokeColor = UIColor.White.CGColor;
            first.LineWidth = (nfloat)1;
            first.FillColor = UIColor.Clear.CGColor;
            PhoneButton.Layer.AddSublayer(first);

            SharingButton.TitleLabel.TextAlignment = UITextAlignment.Center;
            SharingButton.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
            SharingButton.TitleLabel.Lines = 2;              Timer.Elapsed -= Timer_Elapsed;             Timer.Elapsed += Timer_Elapsed;             Timer.AutoReset = false;             Timer.Interval = 30000; //30 seconds 
        }          void Timer_Elapsed(object sender, EventArgs e)         {             InvokeOnMainThread(() => {                  StopSharing();             });         }         public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            StopSharing();              ResignActiveNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillResignActiveNotification, (obj) =>
            {                 StopSharing();             });
        }         public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);              if(!DisplayPurposeOnly)                 StartLocationManager();
        }         public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);              StopSharing();              ResignActiveNotification?.Dispose();             ResignActiveNotification = null;
        } 
        public override bool ShowsWhenEmpty()
        {
            return false;
        }
        public override nfloat GetHeight()
        {
            return 175;
        }
        public override void Reset()
        {
            throw new NotImplementedException();
        }
        async public override void LocationManager_AuthorizationChanged(object sender, CLAuthorizationChangedEventArgs e)
        {
            base.LocationManager_AuthorizationChanged(sender, e);
             if (DisplayPurposeOnly)             {                 SharingButton.SetTitle("", new UIControlState());                 return;             }              if (e.Status == CLAuthorizationStatus.NotDetermined)             {                 ServiceRunner.SharedInstance.StartService<GeolocatorService>();                 var service = ServiceRunner.SharedInstance.FetchService<GeolocatorService>();                 await service.AskPermissionIfNecessary(new Permission[] { Permission.Location });                 return;             }             else if (e.Status == CLAuthorizationStatus.Denied)
            {
                SharingButton.SetTitle(Strings.Sharing.location_permission_necessary, new UIControlState());
            }              else              {
                StopSharing();             }
        }         public void Update()
        {             var title = SelectedCard.Outlets.Any(c => !c.Omitted) ? Strings.Sharing.tap_to_share : Strings.Sharing.select_at_least_one;             SharingButton.SetTitle(title, new UIControlState());         }          partial void BackgroundClicked(Foundation.NSObject sender)
        {             ButtonClicked();         }
        partial void ChangeSharingStatusButtonClicked(Foundation.NSObject sender)         {
            ButtonClicked();
        }          partial void SharingButtonClicked(Foundation.NSObject sender)
        {
            ButtonClicked();         }          void ButtonClicked()
        {             if (DisplayPurposeOnly)
            {                 FirstTapInitiatedAction?.Invoke();                 return;             }              if (!IsSufficentPermissionGranted())             {                 UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(UIApplication.OpenSettingsUrlString));                 return;             }              //make sure at least one outlet is selected             if (!SelectedCard.Outlets.Any(c => !c.Omitted)) return;               if (ButtonLocked) return;             ButtonLocked = true;              if (Sharing)                 StopSharing();             else                 StartSharing();         }         async public void StartSharing()         {             if (DisplayPurposeOnly) return;
            if (SelectedCard == null) return;             if (!IsSufficentPermissionGranted()) return;              SharingButton.SetTitle(Strings.Sharing.authenticating, new UIControlState());
             try
            {
                SessionUUID = await RealmServices.BoardcastCard(SelectedCard, SessionUUID);                 Sharing = !String.IsNullOrEmpty(SessionUUID);             }
            catch (Exception e)
            {
                Sharing = false;                 AppCenterManager.Report(e);
            }              if (Sharing)
            {                 Timer.Start();
                ApplyAnimation();             }
            else
            {
                SharingButton.SetTitle(Strings.Sharing.could_not_share_card, new UIControlState());
            }              ButtonLocked = false;
        }         public void StopSharing()         {             Sharing = false;              if (shape != null)             {                 shape.RemoveAllAnimations();                 shape.RemoveFromSuperLayer();             }
              if(DisplayPurposeOnly)             {                 SharingButton.SetTitle("", new UIControlState());             }
            else if (!IsSufficentPermissionGranted())
            {
                SharingButton.SetTitle(Strings.Sharing.location_permission_necessary, new UIControlState());
            }
            else
            {                 SharingButton.SetTitle(Strings.Sharing.tap_to_share, new UIControlState());                  if (String.IsNullOrEmpty(SessionUUID)) return; 
                Task.Run(async () =>
                {                     if (String.IsNullOrEmpty(SessionUUID)) return;                     await WebServices.TransactionsController.TerminateTransaction(SessionUUID);                 });
            }              ButtonLocked = false;              Timer.Stop();
        }         void ApplyAnimation()
        {             //already started             if (PhoneButton.Layer.Sublayers.Count() > 2) return;              
            shape = new CAShapeLayer();
            shape.Bounds = new CGRect(0, 0, PhoneButton.Frame.Width, PhoneButton.Frame.Height);
            shape.Position = new CGPoint(PhoneButton.Frame.Width / 2, PhoneButton.Frame.Height / 2);
            shape.Path = UIBezierPath.FromOval(PhoneButton.Bounds).CGPath;
            shape.StrokeColor = UIColor.White.CGColor;
            shape.LineWidth = (nfloat).5;
            shape.FillColor = UIColor.Clear.CGColor;
            PhoneButton.Layer.AddSublayer(shape);

            CABasicAnimation grow = CABasicAnimation.FromKeyPath("transform.scale");
            grow.From = NSObject.FromObject(0);
            grow.Duration = 2;
            grow.To = NSObject.FromObject(3);
            grow.FillMode = CAFillMode.Forwards;
            grow.RepeatCount = 10000;
            grow.RemovedOnCompletion = false;
            shape.AddAnimation(grow, "grow");              SharingButton.SetTitle("Sharing", new UIControlState());         }          protected override void Dispose(bool disposing)         {             base.Dispose(disposing);             if (disposing)             {                 shape?.Dispose();                 shape = null;                  ResignActiveNotification?.Dispose();                 ResignActiveNotification = null;                  FirstTapInitiatedAction = null;                  Timer?.Stop();                 Timer?.Dispose();                 Timer = null;             }         }     } }