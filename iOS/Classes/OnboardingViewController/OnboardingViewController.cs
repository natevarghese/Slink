using System;
using CoreGraphics;
using UIKit;
using System.Linq;
using System.Threading.Tasks;
using CoreAnimation;
using Foundation;
using UserNotifications;

namespace Slink.iOS
{
    public class OnboardingViewController : ActivatorContainerViewController
    {
        double animationDuration = 1.8;
        UIImageView SlinkLogoImageView, CardImageView;
        CardViewController CardVC;
        bool Cancelled, LocationPermissionAlertShown, PushNotDetermined;
        NSObject DidBecomeActiveNotificaion;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            //if user has no cards, create their facebook one
            var me = RealmUserServices.GetMe(false);
            if (me.Cards.Count() == 0)
            {
                var outlet = me.Outlets.Where(c => c.Type.Equals(Outlet.outlet_type_facebook, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (outlet != null)
                {
                    var card = Card.Create();
                    card.AddOutlet(outlet);

                    var realm = RealmManager.SharedInstance.GetRealm(null);
                    realm.Write(() =>
                    {
                        card.Name = "Facebook Card";
                        card.Owner = me;
                    });
                }
            }


            NavigationItem.RightBarButtonItem = new UIBarButtonItem(Strings.Basic.skip, UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                Cancelled = true;

                removeAllSubviews(false, () =>
                {
                    //ApplicationExtensions.LoadStoryboardRoot("Landing", false);
                    ApplicationExtensions.EnterApplication(false, true);
                });
            });
        }
        async public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            StartAnimationStory();

            DidBecomeActiveNotificaion = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, (obj) =>
            {
                if (!LocationPermissionAlertShown) return;

                StartPartFourHelper();
                LocationPermissionAlertShown = false;
            });

            var pushSettings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();
            PushNotDetermined = pushSettings.AuthorizationStatus == UNAuthorizationStatus.NotDetermined;
        }
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            DidBecomeActiveNotificaion?.Dispose();
            DidBecomeActiveNotificaion = null;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                SlinkLogoImageView?.Dispose();
                SlinkLogoImageView = null;

                CardImageView?.Dispose();
                CardImageView = null;

                CardVC?.View?.RemoveFromSuperview();
                CardVC?.RemoveFromParentViewController();
                CardVC.Dispose();
                CardVC = null;

                DidBecomeActiveNotificaion?.Dispose();
                DidBecomeActiveNotificaion = null;
            }
        }

        void StartAnimationStory()
        {
            //StartPartFour(null);

            StartPartOne(() =>
            {
                if (Cancelled) return;
                StartPartTwo(() =>
                {
                    if (Cancelled) return;
                    StartPartThree(() =>
                    {
                        //see DidBecomeActive for callback
                    });
                });
            });
        }

        #region PartOne
        void StartPartOne(Action completed)
        {
            SlinkLogoImageView = new UIImageView(new CGRect(View.Frame.GetMidX() - 50, View.Frame.GetMidY() - 100, 100, 100));
            SlinkLogoImageView.Image = UIImage.FromBundle("Logo");
            SlinkLogoImageView.Alpha = 0;
            View.AddSubview(SlinkLogoImageView);

            var hiLabel = GetDefaultLabel(Strings.Onboarding.hi_im_slink);
            hiLabel.Frame = new CGRect(View.Frame.GetMidX() - 100, View.Frame.GetMidY() + 15, 200, 25);
            View.AddSubview(hiLabel);

            var explinationLabel = GetDefaultLabel(Strings.Onboarding.im_here_to_help);
            explinationLabel.Frame = new CGRect(View.Frame.GetMidX() - 100, View.Frame.GetMidY() + 25, 200, 100);
            View.AddSubview(explinationLabel);

            if (Cancelled) return;

            UIView.Animate(animationDuration / 2, () =>
            {
                SlinkLogoImageView.Alpha = 1;
            }, () =>
            {
                if (Cancelled) return;
                UIView.Animate(animationDuration / 2, () =>
                {
                    hiLabel.Alpha = 1;
                }, async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    if (Cancelled) return;
                    UIView.Animate(animationDuration / 2, () =>
                    {
                        explinationLabel.Alpha = 1;
                    }, async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        if (Cancelled) return;
                        MoveLogoToTopLeft();

                        removeAllSubviews(true, () =>
                        {
                            completed?.Invoke();
                        });
                    });
                });
            });
        }
        void MoveLogoToTopLeft()
        {
            if (Cancelled) return;

            var rotate = CABasicAnimation.FromKeyPath("transform.rotation");
            rotate.From = NSObject.FromObject(0);
            rotate.To = NSObject.FromObject(Math.PI * 2);

            var liftAndSlide = CABasicAnimation.FromKeyPath("position");
            liftAndSlide.From = NSValue.FromCGPoint(SlinkLogoImageView.Layer.Position);
            liftAndSlide.To = NSValue.FromCGPoint(new CGPoint(40, 75));

            var shrink = CABasicAnimation.FromKeyPath("transform.scale");
            shrink.To = NSNumber.FromDouble(0.5);

            var animationGroup = CAAnimationGroup.CreateAnimation();
            animationGroup.Animations = new CAAnimation[] { rotate, liftAndSlide, shrink };
            animationGroup.Duration = animationDuration / 2;
            animationGroup.FillMode = CAFillMode.Forwards;
            animationGroup.RemovedOnCompletion = false;
            rotate.RepeatCount = 0;

            SlinkLogoImageView.Layer.AddAnimation(animationGroup, "MoveLogoToTopLeft");
        }
        #endregion

        #region PartTwo
        void StartPartTwo(Action completed)
        {
            if (Cancelled) return;

            var beingBy = GetDefaultLabel(Strings.Onboarding.begin_by);
            beingBy.Frame = new CGRect(100, 50, 200, 25);
            beingBy.Alpha = 0;
            View.AddSubview(beingBy);

            var card = new Card();
            card.UUID = Guid.NewGuid().ToString();
            card.Name = Strings.Basic.new_card;

            CardVC = new CardViewController();
            CardVC.Editable = true;
            CardVC.SelectedCard = card;
            CardVC.View.Frame = new CGRect(0, 100, View.Frame.Width, CardViewController.GetCalculatedHeight());
            CardVC.View.Alpha = 0;
            AddChildViewController(CardVC);
            View.AddSubview(CardVC.View);
            CardVC.AddUserImage(null);

            var nameAndImage = GetDefaultLabel(Strings.Onboarding.add_your_name_and_image);
            nameAndImage.Frame = new CGRect(20, CardVC.View.Frame.GetMaxY() + 50, View.Frame.Width - 40, 100);
            View.AddSubview(nameAndImage);

            UIView.Animate(animationDuration / 2, () =>
            {
                beingBy.Alpha = 1;
            }, () =>
            {
                if (Cancelled) return;
                UIView.Animate(animationDuration / 2, () =>
                {
                    CardVC.View.Alpha = 1;
                }, async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(animationDuration / 2));
                    if (Cancelled) return;
                    UIView.Animate(animationDuration, () =>
                    {
                        nameAndImage.Alpha = 1;
                    }, () =>
                    {
                        AnimateUserNameAndPicture(async () =>
                        {
                            await Task.Delay(TimeSpan.FromSeconds(animationDuration / 2));
                            if (Cancelled) return;
                            UIView.Animate(animationDuration, () =>
                            {
                                nameAndImage.Alpha = 0;
                            }, () =>
                            {
                                nameAndImage.Text = Strings.Onboarding.color_border;
                                if (Cancelled) return;
                                UIView.Animate(animationDuration, () =>
                                {
                                    nameAndImage.Alpha = 1;
                                }, async () =>
                                {
                                    ChangeBorderColor(UIColor.Blue);
                                    await Task.Delay(TimeSpan.FromSeconds(animationDuration / 2));
                                    if (Cancelled) return;
                                    UIView.Animate(animationDuration, () =>
                                    {
                                        nameAndImage.Alpha = 0;
                                    }, () =>
                                    {
                                        nameAndImage.Text = Strings.Onboarding.add_phone_number;
                                        if (Cancelled) return;
                                        UIView.Animate(animationDuration, () =>
                                        {
                                            nameAndImage.Alpha = 1;
                                        }, async () =>
                                        {
                                            AddEmptyPhoneNumber();
                                            await Task.Delay(TimeSpan.FromSeconds(animationDuration / 2));
                                            if (Cancelled) return;
                                            UIView.Animate(animationDuration, () =>
                                            {
                                                nameAndImage.Alpha = 0;
                                            }, () =>
                                            {
                                                nameAndImage.Text = Strings.Onboarding.add_social_media;
                                                if (Cancelled) return;
                                                UIView.Animate(animationDuration, () =>
                                                {
                                                    nameAndImage.Alpha = 1;
                                                }, async () =>
                                                {
                                                    AddEmptySocialMedia();
                                                    await Task.Delay(TimeSpan.FromSeconds(animationDuration / 2));
                                                    if (Cancelled) return;
                                                    UIView.Animate(animationDuration, () =>
                                                    {
                                                        nameAndImage.Alpha = 0;
                                                    }, () =>
                                                    {
                                                        nameAndImage.Text = Strings.Onboarding.add_a_name;
                                                        if (Cancelled) return;
                                                        UIView.Animate(animationDuration, () =>
                                                        {
                                                            nameAndImage.Alpha = 1;
                                                        }, () =>
                                                        {
                                                            AnimateName(() =>
                                                            {
                                                                CardImageView = CardVC.CreateSnapshot();
                                                                removeAllSubviews(true, () =>
                                                                {
                                                                    completed?.Invoke();
                                                                });
                                                            });
                                                        });
                                                    });
                                                });
                                            });
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
            });
        }
        void AnimateUserNameAndPicture(Action completed)
        {
            if (Cancelled) return;

            var me = RealmUserServices.GetMe(false);
            if (me == null) return;

            var targetName = String.IsNullOrEmpty(me.Name) ? Strings.Onboarding.default_name : me.Name;

            var vc = ChildViewControllers.Where(c => c.GetType() == typeof(CardViewController)).First() as CardViewController;
            vc.AnimateTextUserNameLabel(targetName, 0.1, () =>
            {
                vc.AddUserImage(me.RemoteProfileImageURL);
                completed?.Invoke();
            });
        }
        void AnimateName(Action completed)
        {
            if (Cancelled) return;

            var vc = ChildViewControllers.Where(c => c.GetType() == typeof(CardViewController)).First() as CardViewController;
            vc.AnimateTextNameLabel("Social", 0.1, () =>
            {
                completed?.Invoke();
            });
        }
        void ChangeBorderColor(UIColor color)
        {
            if (Cancelled) return;

            var vc = ChildViewControllers.Where(c => c.GetType() == typeof(CardViewController)).First() as CardViewController;
            vc.ChangeBorderColor(color);
        }
        void AddEmptyPhoneNumber()
        {
            if (Cancelled) return;

            var vc = ChildViewControllers.Where(c => c.GetType() == typeof(CardViewController)).First() as CardViewController;
            vc.AddEmptyPhoneNumber();
        }
        void AddEmptySocialMedia()
        {
            if (Cancelled) return;

            var vc = ChildViewControllers.Where(c => c.GetType() == typeof(CardViewController)).First() as CardViewController;
            vc.AddEmptySocialMedia();
        }
        #endregion

        #region PartThree
        void StartPartThree(Action completed)
        {
            if (Cancelled) return;

            var share_with_people_nearby = GetDefaultLabel(Strings.Onboarding.share_with_people_nearby);
            share_with_people_nearby.Frame = new CGRect(100, 50, 200, 75);
            share_with_people_nearby.Alpha = 1;
            View.AddSubview(share_with_people_nearby);

            var whitePhone = new UIImageView(UIImage.FromBundle("PhoneWhiteLarge"));
            whitePhone.ContentMode = UIViewContentMode.ScaleToFill;
            whitePhone.Frame = new CGRect(View.Frame.GetMaxX() - 120, View.Frame.GetMidY() - 100, 100, 210);
            whitePhone.Alpha = 0;
            View.AddSubview(whitePhone);

            var fakeCardView = CardImageView;
            fakeCardView.Alpha = 1;
            View.AddSubview(fakeCardView);

            var arrowImageView = new UIImageView(UIImage.FromBundle("ArrowWhite"));
            arrowImageView.Alpha = 1;
            View.AddSubview(arrowImageView);

            var pressAndHold = GetDefaultLabel(Strings.Onboarding.tap_to_share);
            pressAndHold.Alpha = 1;
            View.AddSubview(pressAndHold);

            CardSharingStatusViewController vc = new CardSharingStatusViewController();
            vc.DisplayPurposeOnly = true;
            vc.View.Frame = new CGRect(50, View.Frame.GetMaxY() - 180, 100, 150);
            vc.View.Alpha = 1;
            AddChildViewController(vc);
            View.AddSubview(vc.View);
            vc.FirstTapInitiatedAction += () =>
            {
                if (Cancelled) return;

                UIView.Animate(animationDuration / 2, () =>
                {
                    whitePhone.Alpha = 1;
                    arrowImageView.Alpha = 0;
                    pressAndHold.Alpha = 0;
                }, async () =>
                {
                    if (Cancelled) return;

                    MoveCardBetweenPhones(fakeCardView, animationDuration, new CGPoint(whitePhone.Frame.GetMidX(), whitePhone.Frame.GetMidY()));
                    await Task.Delay(TimeSpan.FromSeconds(animationDuration));

                    if (IsSufficentPermissionGranted())
                    {
                        StartPartFourHelper();
                    }
                    else if (ArePermissionUnknown()) //show permission alert, first time visitor
                    {
                        LocationPermissionAlertShown = true;
                        vc.StartLocationManager(); //see DidBecomeActive for callback
                    }
                    else //returning user, alert already shown some time ago
                    {
                        StartPartFourHelper();
                    }
                });
            };

            arrowImageView.Frame = new CGRect(vc.View.Frame.GetMaxX() - 20, vc.View.Frame.GetMidY() + 20, 100, 28);
            pressAndHold.Frame = new CGRect(arrowImageView.Frame.GetMidX() - 50, arrowImageView.Frame.GetMaxY(), 100, 25);
            fakeCardView.Frame = new CGRect(vc.View.Center.X - 7, vc.View.Center.Y + 40, 14, 8);
            ShakeViewLeftAndRight(arrowImageView);

        }
        void ShakeViewLeftAndRight(UIView view)
        {
            if (Cancelled) return;

            var shake = CABasicAnimation.FromKeyPath("position");
            shake.Duration = animationDuration;
            shake.FillMode = CAFillMode.Forwards;
            shake.RemovedOnCompletion = false;
            shake.RepeatCount = 9999;
            shake.AutoReverses = true;
            shake.From = NSValue.FromCGPoint(new CGPoint(view.Frame.GetMidX() - 10, view.Frame.GetMidY()));
            shake.To = NSValue.FromCGPoint(new CGPoint(view.Frame.GetMidX() + 10, view.Frame.GetMidY()));
            view.Layer.AddAnimation(shake, "Shake");
        }
        void MoveCardBetweenPhones(UIView card, double duration, CGPoint destination)
        {
            if (Cancelled) return;

            var rotate = CABasicAnimation.FromKeyPath("transform.rotation");
            rotate.From = NSObject.FromObject(0);
            rotate.To = NSObject.FromObject(Math.PI * 2);

            var liftAndSlideRight = CABasicAnimation.FromKeyPath("position");
            liftAndSlideRight.To = NSValue.FromCGPoint(destination);
            liftAndSlideRight.RemovedOnCompletion = false;
            liftAndSlideRight.FillMode = CAFillMode.Forwards;

            var grow = CABasicAnimation.FromKeyPath("transform.scale");
            grow.To = NSNumber.FromDouble(7.5);
            grow.Duration = animationDuration / 2;
            grow.RemovedOnCompletion = false;
            grow.FillMode = CAFillMode.Forwards;

            var shrink = CABasicAnimation.FromKeyPath("transform.scale");
            shrink.To = NSNumber.FromDouble(3.0);
            shrink.BeginTime = animationDuration / 2;
            shrink.RemovedOnCompletion = false;
            shrink.FillMode = CAFillMode.Forwards;

            var animationGroup = CAAnimationGroup.CreateAnimation();
            animationGroup.Animations = new CAAnimation[] { rotate, liftAndSlideRight, grow, shrink };
            animationGroup.Duration = duration;
            animationGroup.FillMode = CAFillMode.Forwards;
            animationGroup.RemovedOnCompletion = false;
            rotate.RepeatCount = 0;

            card.Layer.AddAnimation(animationGroup, "MoveCardBetweenPhones");
        }
        #endregion

        #region StartPartFour
        void StartPartFour(Action completed)
        {
            if (Cancelled) return;

            var get_notified_label = GetDefaultLabel(Strings.Onboarding.get_notified);
            get_notified_label.Frame = new CGRect(100, 50, 200, 70);
            get_notified_label.Alpha = 0;
            View.AddSubview(get_notified_label);

            var redDot = new UIView();
            redDot.Frame = new CGRect(20, 120, 20, 20);
            redDot.Alpha = 0;
            redDot.BackgroundColor = UIColor.Red;
            redDot.Layer.MasksToBounds = true;
            redDot.Layer.CornerRadius = redDot.Layer.Bounds.Width / 2;
            View.AddSubview(redDot);

            var redDotTwo = new UIView();
            redDotTwo.Frame = new CGRect(20, 120, 20, 20);
            redDotTwo.Alpha = 0;
            redDotTwo.BackgroundColor = UIColor.Red;
            redDotTwo.Layer.MasksToBounds = true;
            redDotTwo.Layer.CornerRadius = redDotTwo.Layer.Bounds.Width / 2;
            View.AddSubview(redDotTwo);

            var redDotThree = new UIView();
            redDotThree.Frame = new CGRect(20, 120, 20, 20);
            redDotThree.Alpha = 0;
            redDotThree.BackgroundColor = UIColor.Red;
            redDotThree.Layer.MasksToBounds = true;
            redDotThree.Layer.CornerRadius = redDotThree.Layer.Bounds.Width / 2;
            View.AddSubview(redDotThree);

            var whitePhone = new UIImageView(UIImage.FromBundle("PhoneWhiteLarge"));
            whitePhone.ContentMode = UIViewContentMode.ScaleToFill;
            whitePhone.Frame = new CGRect(View.Frame.GetMaxX() - 120, View.Frame.GetMidY() - 100, 100, 210);
            whitePhone.Alpha = 0;
            View.AddSubview(whitePhone);

            var arrowImageView = new UIImageView(UIImage.FromBundle("ArrowWhite"));
            arrowImageView.Transform = CGAffineTransform.MakeRotation(1.5708f);
            arrowImageView.Frame = new CGRect(View.Frame.GetMaxX() - 70, 40, 40, 100);
            arrowImageView.Alpha = 0;
            View.AddSubview(arrowImageView);

            var enablePushNotificaionsButton = new UIButton(UIButtonType.System);
            enablePushNotificaionsButton.SetTitle("Enable\nPush Notificaions", new UIControlState());
            enablePushNotificaionsButton.SetTitleColor(UIColor.White, new UIControlState());
            enablePushNotificaionsButton.TitleLabel.Font = UIFont.FromName("Avenir-Medium", 18);
            enablePushNotificaionsButton.TitleLabel.Lines = 0;
            enablePushNotificaionsButton.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
            enablePushNotificaionsButton.TitleLabel.TextAlignment = UITextAlignment.Center;
            enablePushNotificaionsButton.Frame = new CGRect(20, View.Frame.GetMidY(), whitePhone.Frame.X - 20, 50);
            enablePushNotificaionsButton.Alpha = 0;
            enablePushNotificaionsButton.TouchUpInside += (sender, e) =>
            {
                if (!PushNotDetermined) return;

                var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
                appDelegate.SetUpPushNotificaions();
                enablePushNotificaionsButton.Alpha = 0;
            };
            View.AddSubview(enablePushNotificaionsButton);


            UIView.Animate(animationDuration, () =>
            {
                get_notified_label.Alpha = 1;
                whitePhone.Alpha = 1;
                redDot.Alpha = 1;
                redDotTwo.Alpha = 1;
                redDotThree.Alpha = 1;
                enablePushNotificaionsButton.Alpha = PushNotDetermined ? 1 : 0;
            }, () =>
            {
                if (Cancelled) return;

                UIView.Animate(animationDuration / 3, () =>
                {
                    redDot.Frame = new CGRect(whitePhone.Center, redDot.Frame.Size);
                }, () =>
                {
                    if (Cancelled) return;

                    UIView.Animate(animationDuration / 3, () =>
                    {
                        redDotTwo.Frame = new CGRect(whitePhone.Center, redDotTwo.Frame.Size);
                    }, () =>
                    {
                        if (Cancelled) return;

                        UIView.Animate(animationDuration / 3, () =>
                        {
                            redDotThree.Frame = new CGRect(whitePhone.Center, redDotThree.Frame.Size);
                        }, () =>
                        {
                            if (Cancelled) return;

                            UIView.Animate(animationDuration, () =>
                            {
                                get_notified_label.Alpha = 0;
                                whitePhone.Alpha = 0;
                                redDot.Alpha = 0;
                                redDotTwo.Alpha = 0;
                                redDotThree.Alpha = 0;

                                //todo uncomment
                                //SlinkLogoImageView.Alpha = 0;
                            }, () =>
                                {
                                    arrowImageView.Alpha = 1;
                                    NavigationItem.RightBarButtonItem = new UIBarButtonItem(Strings.Basic.lets_go, UIBarButtonItemStyle.Plain, (sender, e) =>
                                    {
                                        Cancelled = true;

                                        removeAllSubviews(false, () =>
                                        {
                                            ApplicationExtensions.EnterApplication(false, true);
                                        });
                                    });
                                });
                        });
                    });
                });
            });

        }
        void StartPartFourHelper()
        {
            if (Cancelled) return;

            removeAllSubviews(true, () =>
            {
                StartPartFour(() =>
                {
                    if (Cancelled) return;
                });
            });

            CardVC?.RemoveFromParentViewController();
        }
        #endregion

        void removeAllSubviews(bool animated, Action completed)
        {
            if (animated)
            {
                UIView.Animate(animationDuration, () =>
                {
                    View.Subviews.Where(v => v != SlinkLogoImageView).ToList().ForEach(v => v.Alpha = 0);
                }, () =>
                {
                    removeAllSubviews(false, completed);
                });
            }
            else
            {
                View.Subviews.Where(v => v != SlinkLogoImageView).ToList().ForEach(v => v.RemoveFromSuperview());
                completed?.Invoke();
            }
        }
        UILabel GetDefaultLabel(string text)
        {
            var label = new UILabel();
            label.Text = text;
            label.TextAlignment = UITextAlignment.Center;
            label.Lines = 0;
            label.LineBreakMode = UILineBreakMode.WordWrap;
            label.TextColor = UIColor.White;
            label.Alpha = 0;
            label.Font = UIFont.FromName("Avenir-Medium", 17);

            return label;
        }
    }
}
