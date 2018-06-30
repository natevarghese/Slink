using System; using CoreGraphics; using Foundation; using UIKit; using System.Linq; using System.Threading.Tasks;
using Google.MobileAds;
using Facebook.CoreKit; using System.Collections.Generic;
using Plugin.Geolocator;

namespace Slink.iOS {
    public partial class FlyingObjectsContainterViewController : UIViewController
    {
        FlyingObjectsView FlyingObjects;
        NSObject DidEnterBackgroundToken, WillEnterForgroundToken, DesignChangedToken;
        nfloat AddViewSuperviewHeight = 50;          string AdKeyGender = "gender";         string AdKeyBirthday = "birthday";          public ClearNavigationController ContainerNavigationController = new ClearNavigationController();
        public bool ShowsAds = true;          public FlyingObjectsContainterViewController(IntPtr handle) : base(handle) { }
        public FlyingObjectsContainterViewController() : base("FlyingObjectsContainterViewController", null) { }

        public static FlyingObjectsContainterViewController CreateModal(UIViewController nestedViewController)
        {
            var flyingObjectsContainerViewController = new FlyingObjectsContainterViewController();
            flyingObjectsContainerViewController.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;

            var clearNavigationController = flyingObjectsContainerViewController.ContainerNavigationController;
            clearNavigationController.SetViewControllers(new UIViewController[] { nestedViewController }, false);

            return flyingObjectsContainerViewController;
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad(); 
            FlyingObjects = new FlyingObjectsView(View.Frame);             View.InsertSubview(FlyingObjects, 0); 
            AddChildViewController(ContainerNavigationController);
            ContainerView.AddSubview(ContainerNavigationController.View);
            ContainerView.AddConstraint(NSLayoutConstraint.Create(ContainerNavigationController.View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Top, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(ContainerNavigationController.View, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Right, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(ContainerNavigationController.View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Bottom, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(ContainerNavigationController.View, NSLayoutAttribute.Left, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Left, 1, 0));             ContainerNavigationController.View.TranslatesAutoresizingMaskIntoConstraints = false;              ContainerViewTopMarginConstraint.Constant = UIApplication.SharedApplication.StatusBarFrame.Height;                      }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            WillEnterForgroundToken = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, (obj) =>
            {                 StartFlyingObjectsView();
            });

            DidEnterBackgroundToken = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidEnterBackgroundNotification, (obj) =>
            {
                FlyingObjects.EndAnimation(FlyingObjectsView.AnimationEnding.collapse, 0.1f);
            });              DesignChangedToken = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(Strings.InternalNotifications.notification_design_changed), async (obj) =>             {
                FlyingObjects.EndAnimation(FlyingObjectsView.AnimationEnding.collapse, .01f);                 await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.5));                 StartFlyingObjectsView();
            });              AdViewSuperViewHeightConstaint.Constant = 0;             if (ShowsAds)             {                 var iPersistant = ServiceLocator.Instance.Resolve<IPersistantStorage>();                 var facebookToken = iPersistant.GetFacebookToken();                                      var graphRequest = new GraphRequest("/me?fields=gender,birthday", null, facebookToken, null, "GET");                 var requestConnection = new GraphRequestConnection();                 requestConnection.AddRequest(graphRequest, (connection, result, error) =>                 {                     var data = result as NSDictionary;                     if (data == null) return;                      var dict = new Dictionary<string, string>();                      if(data.ContainsKey(new NSString(AdKeyGender)))                         dict.Add(AdKeyGender, data[AdKeyGender].ToString());                      if(data.ContainsKey(new NSString(AdKeyBirthday)))                         dict.Add(AdKeyBirthday, data[AdKeyBirthday].ToString());                                          ShowBanner(dict);                 });                 requestConnection.Start();             }
        }          public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (!FlyingObjects.Animating)
                StartFlyingObjectsView();         }         public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (FlyingObjects.Animating)
                FlyingObjects.EndAnimation(FlyingObjectsView.AnimationEnding.collapse, 0.1f);
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            DidEnterBackgroundToken?.Dispose();
            DidEnterBackgroundToken = null;

            WillEnterForgroundToken?.Dispose();
            WillEnterForgroundToken = null;              DesignChangedToken?.Dispose();             DesignChangedToken = null;
        }         public override void ViewWillLayoutSubviews()         {             base.ViewWillLayoutSubviews();              if (FlyingObjects != null)                 FlyingObjects.Frame = View.Frame;         }         void StartFlyingObjectsView()
        {             var service = ServiceLocator.Instance.Resolve<IPersistantStorage>();             var designType = service.GetDesignType();             FlyingObjects.StartAnimationLoop(FlyingObjectsView.AnimationDirection.left, designType, 20, 100, 1);          }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                FlyingObjects?.Dispose();
                FlyingObjects = null;

                ContainerNavigationController?.Dispose();
                ContainerNavigationController = null;

                DidEnterBackgroundToken?.Dispose();
                DidEnterBackgroundToken = null;

                WillEnterForgroundToken?.Dispose();
                WillEnterForgroundToken = null;                  DesignChangedToken?.Dispose();                 DesignChangedToken = null;
            }
        }                   void ShowBanner(Dictionary<string, string> advertisingTargetInfo)
        {             var bannerView = new BannerView(AdSizeCons.SmartBannerPortrait);             bannerView.AdUnitID = "ca-app-pub-4252799872870196/8180596755";             bannerView.RootViewController = this;             bannerView.ReceiveAdFailed += (sender, e) =>             {                 Console.Write(e.Error.LocalizedDescription);                 AdViewSuperViewHeightConstaint.Constant = 0;             };             bannerView.AdReceived += (sender, e) =>             {                 AdViewSuperViewHeightConstaint.Constant = AddViewSuperviewHeight;             };             AdViewSuperView.AddSubview(bannerView);              var request = GetRequest(advertisingTargetInfo);             bannerView.LoadRequest(request);         }         Request GetRequest(Dictionary<string, string> advertisingTargetInfo)         {             var request = Request.GetDefaultRequest();             request.TestDevices = new string[] { Request.SimulatorId.ToString() };              //Gender             var gender = Gender.Unknown;             if (advertisingTargetInfo.ContainsKey(AdKeyGender))                 gender = advertisingTargetInfo[AdKeyGender].Equals("female", StringComparison.InvariantCultureIgnoreCase) ? Gender.Female : Gender.Male;             request.Gender = gender;              //Location             var location = RealmServices.GetLastUserLocation();             if (location != null)                 request.SetLocation((nfloat)location.Latitude, (nfloat)location.Longitude, 1);              //Birthday             if (advertisingTargetInfo.ContainsKey(AdKeyBirthday))             {                 var birthday = DateTime.ParseExact(advertisingTargetInfo[AdKeyBirthday], "MM/dd/yyyy",null);                 request.SetBirthday(birthday.Month, birthday.Day, birthday.Year);             }              //todo Keywords              return request;         }
        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }

        public void ShiftContainerRight(nfloat newVal)
        {
            ContainerViewLeftMarginConstraint.Constant = newVal;
            ContainerViewRightMarginConstraint.Constant = -newVal;
        }
    } } 