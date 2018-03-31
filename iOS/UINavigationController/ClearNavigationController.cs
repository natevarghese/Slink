using System;
using CoreAnimation;
using UIKit;

namespace Slink.iOS
{
    public partial class ClearNavigationController : UINavigationController
    {
        public ClearNavigationController(IntPtr handle) : base(handle) { }
        public ClearNavigationController(UIViewController rootViewController) : base(rootViewController) { }
        public ClearNavigationController() : base() { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationBar.ShadowImage = new UIImage();
            NavigationBar.BackgroundColor = UIColor.Clear;
            NavigationBar.Translucent = true;
            NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            NavigationBar.TintColor = UIColor.White;
            NavigationBar.TitleTextAttributes = new UIStringAttributes()
            {
                ForegroundColor = UIColor.White
            };
        }

        public override void PushViewController(UIViewController viewController, bool animated)
        {
            if (animated)
            {
                var transition = CATransition.CreateAnimation();
                transition.Duration = 0.3;
                transition.Type = CATransition.TransitionFade;
                transition.Subtype = CATransition.TransitionFromTop;
                View.Layer.AddAnimation(transition, CALayer.Transition);
            }
            base.PushViewController(viewController, false);
        }


        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

