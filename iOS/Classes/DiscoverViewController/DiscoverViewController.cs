using System;
using CoreGraphics;
using UIKit;
using System.Linq;
using CoreAnimation;
using Foundation;
using CoreLocation;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Slink.iOS
{
    //todo there is a bug when going to this page first on a clean install and the location permission pops up.
    //it doesnt try to search on dismiss
    public partial class DiscoverViewController : BaseViewController
    {
        UIView DemoCardView;
        CardSharingStatusViewController BackgroundVC = new CardSharingStatusViewController();
        CAShapeLayer shape;
        DiscoverShared Shared = new DiscoverShared();

        static bool Searching, ShouldStopSearching;


        public DiscoverViewController() : base("DiscoverViewController") { }

        public override string Title
        {
            get
            {
                return DrawerShared.navigation_item_discover;
            }
            set { base.Title = value; }
        }

        public override void ViewDidLoad()
        {
            LocationEnabled = true;

            base.ViewDidLoad();

            //adds a stationary circle around phone
            CAShapeLayer first = new CAShapeLayer();
            first.Bounds = new CGRect(0, 0, PhoneImageView.Frame.Width, PhoneImageView.Frame.Height);
            first.Position = new CGPoint(PhoneImageView.Frame.Width / 2, PhoneImageView.Frame.Height / 2);
            first.Path = UIBezierPath.FromOval(PhoneImageView.Bounds).CGPath;
            first.StrokeColor = UIColor.White.CGColor;
            first.LineWidth = (nfloat)1;
            first.FillColor = UIColor.Clear.CGColor;
            PhoneImageView.Layer.AddSublayer(first);

            //Wire up events
            DemoCardView = new UIView();
            DemoCardView.BackgroundColor = UIColor.Clear;

            DemoCardSuperviewHeightConstraint.Constant = CardViewController.GetCalculatedHeight();
            DemoCardSuperview.AddSubview(DemoCardView);
            DemoCardSuperview.SetNeedsLayout();
            DemoCardSuperview.LayoutIfNeeded();
            DemoCardSuperview.BackgroundColor = UIColor.Clear;

            StatusButton.Hidden = true;
            StatusButton.TitleLabel.TextAlignment = UITextAlignment.Center;
            StatusButton.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
            StatusButton.TitleLabel.Lines = 2;

            RightArrowImageView.Transform = CGAffineTransform.MakeScale(-1, 1);

            HideCardViews(true);

            //clear notifications
            var persistantStorageService = ServiceLocator.Instance.Resolve<IPersistantStorage>();
            persistantStorageService.SetDiscoverNotificaionCount(0);


            if (IsSufficentPermissionGranted())
            {
                var service = ServiceRunner.SharedInstance.FetchService<GeolocatorService>();
                service.Start();
            }

            StartLocationManager();
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            StartSearching();
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            StopSearching();
        }
        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            BackgroundVC.View.Frame = View.Bounds;

            if (DemoCardView != null)
            {
                DemoCardView.Frame = DemoCardSuperview.Bounds;
                DemoCardView.Bounds = new CGRect(0, 0, View.Bounds.Width - 16, DemoCardSuperview.Bounds.Height);

                if (!DemoCardSuperview.Subviews.Contains(DemoCardView))
                    DemoCardSuperview.AddSubview(DemoCardView);
            }
        }
        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (DemoCardView != null)
            {
                DemoCardView.Frame = DemoCardSuperview.Bounds;
                DemoCardView.Bounds = new CGRect(0, 0, DemoCardSuperview.Bounds.Width, DemoCardSuperview.Bounds.Height);
            }
        }
        async public override void LocationManager_AuthorizationChanged(object sender, CLAuthorizationChangedEventArgs e)
        {
            base.LocationManager_AuthorizationChanged(sender, e);

            if (e == null) return;

            if (e.Status == CLAuthorizationStatus.NotDetermined)
            {
                ServiceRunner.SharedInstance.StartService<GeolocatorService>();
                var service = ServiceRunner.SharedInstance.FetchService<GeolocatorService>();
                await service.AskPermissionIfNecessary(new Permission[] { Permission.Location });
                return;
            }
            if (e.Status == CLAuthorizationStatus.Denied || !IsSufficentPermissionGranted()) return;

            StartLocationManager();
            StartSearching();
        }

        async void StartSearching()
        {
            if (Searching) return;
            Searching = true;
            ShouldStopSearching = false;

            if (!IsSufficentPermissionGranted())
            {
                StatusButton.Hidden = false;
                StatusButton.SetTitle("Location Permission Necessary. \n Go to Settings", new UIControlState());
                return;
            }

            shape = new CAShapeLayer();
            shape.Bounds = new CGRect(0, 0, PhoneImageView.Frame.Width, PhoneImageView.Frame.Height);
            shape.Position = new CGPoint(PhoneImageView.Frame.Width / 2, PhoneImageView.Frame.Height / 2);
            shape.Path = UIBezierPath.FromOval(PhoneImageView.Bounds).CGPath;
            shape.StrokeColor = UIColor.White.CGColor;
            shape.LineWidth = (nfloat).5;
            shape.FillColor = UIColor.Clear.CGColor;
            PhoneImageView.Layer.AddSublayer(shape);

            CABasicAnimation grow = CABasicAnimation.FromKeyPath("transform.scale");
            grow.From = NSObject.FromObject(20);
            grow.Duration = 2;
            grow.To = NSObject.FromObject(1);
            grow.FillMode = CAFillMode.Forwards;
            grow.RepeatCount = 10000;
            grow.RemovedOnCompletion = false;
            shape.AddAnimation(grow, "grow");

            StatusButton.Hidden = false;
            StatusButton.SetTitle("Searching for people \n sharing nearby", new UIControlState());

            while (!ShouldStopSearching && View.Window != null)
            {
                await Shared.GetNearbyTransactions();

                StopSearchingIfCardsFound();

                Console.WriteLine("GOT");
                await Task.Delay(TimeSpan.FromSeconds(5));
                Console.WriteLine("DONE");
            }
        }
        public void StopSearching()
        {
            Searching = false;
            ShouldStopSearching = true;

            StatusButton.Hidden = true;
            StatusButton.SetTitle(null, new UIControlState());

            shape?.RemoveAllAnimations();
            shape?.RemoveFromSuperLayer();

        }


        partial void SatusButtonClicked(Foundation.NSObject sender)
        {
            if (!IsSufficentPermissionGranted())
                UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(UIApplication.OpenSettingsUrlString));
            else
                StartLocationManager();
        }
        partial void FlipButtonClicked(Foundation.NSObject sender)
        {
            Shared.TableItems.First().Card.Flip();

            foreach (var vc in ChildViewControllers.Where(c => c.GetType() == typeof(CardViewController)))
                ((CardViewController)vc).PerformFlipAnimationIfNecessary();
        }
        partial void NoClicked(Foundation.NSObject sender)
        {
            UIView.Animate(0.5, () =>
            {
                var vc = ChildViewControllers.Where(c => c.GetType() == typeof(CardViewController)).FirstOrDefault();
                vc.View.Alpha = 0;
            }, () =>
            {
                Shared.RejectCard();
                RemoveChildCardViewControllers();
                StopSearchingIfCardsFound();
            });
        }
        partial void YesClicked(Foundation.NSObject sender)
        {
            UIView.Animate(0.5, () =>
            {
                var vc = ChildViewControllers.Where(c => c.GetType() == typeof(CardViewController)).FirstOrDefault();
                vc.View.Alpha = 0;
            }, () =>
            {
                Shared.AcceptCard();
                RemoveChildCardViewControllers();
                StopSearchingIfCardsFound();
            });
        }


        void RemoveChildCardViewControllers()
        {
            var vc = ChildViewControllers.Where(c => c.GetType() == typeof(CardViewController)).FirstOrDefault();
            vc.WillMoveToParentViewController(null);
            vc.View.RemoveFromSuperview();
            vc.RemoveFromParentViewController();
        }
        public void NextCardForCardView()
        {
            var item = Shared.GetNextCard();
            if (item == null)
            {
                Console.WriteLine("Item is null");
                return;
            }

            Console.WriteLine("Item is NOT null");

            var vc = new CardViewController();
            vc.HideTitle = true;
            vc.Editable = false;
            vc.SelectedCard = item.Card;
            vc.View.Frame = DemoCardView.Bounds;

            AddChildViewController(vc);
            DemoCardView.AddSubview(vc.View);
            vc.DidMoveToParentViewController(this);
        }
        void StopSearchingIfCardsFound()
        {
            if (Shared.ShouldStopSearching())
            {
                HideCardViews(false);
                StopSearching();

                NextCardForCardView();
            }
            else
            {
                HideCardViews(true);
                StartSearching();
            }
        }
        void HideCardViews(bool hidden)
        {
            nfloat finalResult = (hidden) ? 0 : 1;
            nfloat oppositeResult = (hidden) ? 1 : 0;
            UIView.Animate(0.5, () =>
            {
                DemoCardSuperview.Alpha = finalResult;
                NoLabel.Alpha = finalResult;
                NoBackgroundView.Alpha = finalResult;
                YesLabel.Alpha = finalResult;
                YesBackgroundView.Alpha = finalResult;
                RightArrowImageView.Alpha = finalResult;
                LeftArrowImageView.Alpha = finalResult;
                FlipButton.Alpha = finalResult;


                StatusButton.Alpha = oppositeResult;
                PhoneImageView.Alpha = oppositeResult;

                TitleLabel.Text = (hidden) ? null : Strings.Discover.item_presented;
            });
        }


    }
}