using System;
using CoreGraphics;
using UIKit;
using Foundation;
using System.Collections.Generic;
using System.Linq;
using CoreText;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Com.OneSignal.iOS;

namespace Slink.iOS
{
    public partial class DrawerViewController : UIViewController
    {
        UITapGestureRecognizer TapGestureRecognizerToHideDrawer;
        DrawerTableViewController TableViewController = new DrawerTableViewController();

        public DrawerViewController() : base("DrawerViewController", null) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InititalsLabel.BackgroundColor = UIColor.FromWhiteAlpha(0.9f, 1);
            InititalsLabel.TextColor = UIColor.Gray;
            InititalsLabel.Layer.MasksToBounds = true;
            InititalsLabel.Layer.CornerRadius = InititalsLabel.Frame.Size.Width / 2;




            AddChildViewController(TableViewController);
            ContainerView.AddSubview(TableViewController.View);
            View.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Top, 1, 0));
            View.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Right, 1, 0));
            View.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Bottom, 1, 0));
            View.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Left, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Left, 1, 0));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var me = RealmUserServices.GetMe(true);

            var initials = (me.FirstName?.FirstOrDefault() + " " + me.LastName?.FirstOrDefault()).Trim();
            CTStringAttributes attributes = new CTStringAttributes();
            attributes.KerningAdjustment = -2;
            NSAttributedString attributedString = new NSAttributedString(initials, attributes);

            InititalsLabel.AttributedText = attributedString;

            NameLabel.Text = me.FirstName + " " + me.LastName;
            HandleLabel.Text = me.Handle;

            FooterLabel.Text = DrawerShared.GetFooterText();
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            View.LayoutIfNeeded();

            TapGestureRecognizerToHideDrawer = new UITapGestureRecognizer(Dismiss);
            TapGestureRecognizerToHideDrawer.CancelsTouchesInView = false;
            View.Window.AddGestureRecognizer(TapGestureRecognizerToHideDrawer);
        }
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (TapGestureRecognizerToHideDrawer != null)
            {
                View.Window.RemoveGestureRecognizer(TapGestureRecognizerToHideDrawer);
                TapGestureRecognizerToHideDrawer.Dispose();
                TapGestureRecognizerToHideDrawer = null;
            }
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                TapGestureRecognizerToHideDrawer?.Dispose();
                TapGestureRecognizerToHideDrawer = null;

                TableViewController?.Dispose();
                TableViewController = null;
            }
        }
        partial void HeaderViewClicked(NSObject sender)
        {
            //DismissViewController(true, () =>
            //{
            //    var vc = FlyingObjectsContainterViewController.CreateModal(new EditProfileViewController());
            //    ApplicationExtensions.PresentViewControllerOnRoot(vc, true);
            //});
        }

        public void Dismiss(UIGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer == null)//called dismiss somewhere via code
            {
                DismissViewController(true, null);
                return;
            }

            //if tap was outside DrawerViewController.View
            if (!View.PointInside(View.ConvertPointFromView(gestureRecognizer.LocationInView(null), View.Window), null))
            {
                DismissViewController(true, null);
                return;
            }
        }

    }
}