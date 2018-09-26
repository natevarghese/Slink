// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Slink.iOS
{
    [Register ("DiscoverViewController")]
    partial class DiscoverViewController
    {
        [Outlet]
        UIKit.UIView DemoCardSuperview { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint DemoCardSuperviewHeightConstraint { get; set; }


        [Outlet]
        UIKit.UIButton FlipButton { get; set; }


        [Outlet]
        UIKit.UIImageView LeftArrowImageView { get; set; }


        [Outlet]
        UIKit.UIControl NoBackgroundView { get; set; }


        [Outlet]
        UIKit.UILabel NoLabel { get; set; }


        [Outlet]
        UIKit.UIImageView PhoneImageView { get; set; }


        [Outlet]
        UIKit.UIImageView RightArrowImageView { get; set; }


        [Outlet]
        UIKit.UIButton StatusButton { get; set; }


        [Outlet]
        UIKit.UILabel TitleLabel { get; set; }


        [Outlet]
        UIKit.UIControl YesBackgroundView { get; set; }


        [Outlet]
        UIKit.UILabel YesLabel { get; set; }


        [Action ("FlipButtonClicked:")]
        partial void FlipButtonClicked (Foundation.NSObject sender);


        [Action ("NoClicked:")]
        partial void NoClicked (Foundation.NSObject sender);


        [Action ("SatusButtonClicked:")]
        partial void SatusButtonClicked (Foundation.NSObject sender);


        [Action ("YesClicked:")]
        partial void YesClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (DemoCardSuperview != null) {
                DemoCardSuperview.Dispose ();
                DemoCardSuperview = null;
            }

            if (DemoCardSuperviewHeightConstraint != null) {
                DemoCardSuperviewHeightConstraint.Dispose ();
                DemoCardSuperviewHeightConstraint = null;
            }

            if (FlipButton != null) {
                FlipButton.Dispose ();
                FlipButton = null;
            }

            if (LeftArrowImageView != null) {
                LeftArrowImageView.Dispose ();
                LeftArrowImageView = null;
            }

            if (NoBackgroundView != null) {
                NoBackgroundView.Dispose ();
                NoBackgroundView = null;
            }

            if (NoLabel != null) {
                NoLabel.Dispose ();
                NoLabel = null;
            }

            if (PhoneImageView != null) {
                PhoneImageView.Dispose ();
                PhoneImageView = null;
            }

            if (RightArrowImageView != null) {
                RightArrowImageView.Dispose ();
                RightArrowImageView = null;
            }

            if (StatusButton != null) {
                StatusButton.Dispose ();
                StatusButton = null;
            }

            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }

            if (YesBackgroundView != null) {
                YesBackgroundView.Dispose ();
                YesBackgroundView = null;
            }

            if (YesLabel != null) {
                YesLabel.Dispose ();
                YesLabel = null;
            }
        }
    }
}