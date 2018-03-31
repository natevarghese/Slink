using System;
using CoreGraphics;
using UIKit;

namespace Slink.iOS
{
    public partial class ContainerViewController : BaseViewController
    {
        public UIViewController TargetViewController;

        public ContainerViewController() : base("ContainerViewController") { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AutomaticallyAdjustsScrollViewInsets = false;
            TargetViewController.AutomaticallyAdjustsScrollViewInsets = false;

            if (TargetViewController != null)
            {
                TargetViewController.View.TranslatesAutoresizingMaskIntoConstraints = false;
                AddChildViewController(TargetViewController);
                View.AddSubview(TargetViewController.View);

                View.AddConstraint(NSLayoutConstraint.Create(TargetViewController.View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 44));
                View.AddConstraint(NSLayoutConstraint.Create(TargetViewController.View, NSLayoutAttribute.Right, NSLayoutRelation.Equal, View, NSLayoutAttribute.Right, 1, 0));
                View.AddConstraint(NSLayoutConstraint.Create(TargetViewController.View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0));
                View.AddConstraint(NSLayoutConstraint.Create(TargetViewController.View, NSLayoutAttribute.Left, NSLayoutRelation.Equal, View, NSLayoutAttribute.Left, 1, 0));
            }


        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

