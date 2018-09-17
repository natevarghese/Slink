using System;
using System.Threading.Tasks;
using System.Linq;
using CoreGraphics;
using UIKit;

namespace Slink.iOS
{
    public partial class ActivatorViewController : BaseViewController
    {
        UIViewController ActiveViewController;

        public ActivatorViewController() : base() { }
        public ActivatorViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            NetworkListenerEnabled = false;

            base.ViewDidLoad();
        }
        public void SetViewController(UIViewController vc, bool animated)
        {
            //dont activate new vc instance of same type
            var target = ChildViewControllers.FirstOrDefault();
            if (target != null)
            {
                if (target.GetType() == vc.GetType())
                    return;
            }


            vc.View.TranslatesAutoresizingMaskIntoConstraints = false;

            if (animated)
            {
                UIView.Transition(View, 1.0, UIViewAnimationOptions.CurveEaseOut, () =>
                {
                    View.Subviews?.ToList().ForEach(p => p.RemoveFromSuperview());
                    ChildViewControllers?.ToList().ForEach(p => p.RemoveFromParentViewController());

                    AddChildViewController(vc);
                    View.AddSubview(vc.View);
                    View.AddConstraint(NSLayoutConstraint.Create(vc.View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0));
                    View.AddConstraint(NSLayoutConstraint.Create(vc.View, NSLayoutAttribute.Right, NSLayoutRelation.Equal, View, NSLayoutAttribute.Right, 1, 0));
                    View.AddConstraint(NSLayoutConstraint.Create(vc.View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0));
                    View.AddConstraint(NSLayoutConstraint.Create(vc.View, NSLayoutAttribute.Left, NSLayoutRelation.Equal, View, NSLayoutAttribute.Left, 1, 0));
                }, null);
            }
            else
            {

                View.Subviews?.ToList().ForEach(p => p.RemoveFromSuperview());
                ChildViewControllers?.ToList().ForEach(p => p.RemoveFromParentViewController());

                AddChildViewController(vc);
                View.AddSubview(vc.View);
                View.AddConstraint(NSLayoutConstraint.Create(vc.View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0));
                View.AddConstraint(NSLayoutConstraint.Create(vc.View, NSLayoutAttribute.Right, NSLayoutRelation.Equal, View, NSLayoutAttribute.Right, 1, 0));
                View.AddConstraint(NSLayoutConstraint.Create(vc.View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0));
                View.AddConstraint(NSLayoutConstraint.Create(vc.View, NSLayoutAttribute.Left, NSLayoutRelation.Equal, View, NSLayoutAttribute.Left, 1, 0));
            }

            ActiveViewController = vc;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                ActiveViewController?.Dispose();
                ActiveViewController = null;
            }
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

