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
    [Register ("DrawerTableViewCell")]
    partial class DrawerTableViewCell
    {
        [Outlet]
        Slink.iOS.WebImage LeftImageView { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint LeftImageViewWidthConstraint { get; set; }


        [Outlet]
        UIKit.UILabel LeftLabel { get; set; }


        [Outlet]
        UIKit.UILabel NotificaionView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (LeftImageView != null) {
                LeftImageView.Dispose ();
                LeftImageView = null;
            }

            if (LeftImageViewWidthConstraint != null) {
                LeftImageViewWidthConstraint.Dispose ();
                LeftImageViewWidthConstraint = null;
            }

            if (LeftLabel != null) {
                LeftLabel.Dispose ();
                LeftLabel = null;
            }

            if (NotificaionView != null) {
                NotificaionView.Dispose ();
                NotificaionView = null;
            }
        }
    }
}