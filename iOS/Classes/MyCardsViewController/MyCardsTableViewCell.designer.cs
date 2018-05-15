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
    [Register ("MyCardsTableViewCell")]
    partial class MyCardsTableViewCell
    {
        [Outlet]
        UIKit.UIView ContainerView { get; set; }


        [Outlet]
        UIKit.UILabel NameLabel { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint NameLabelHeightConstraint { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ContainerView != null) {
                ContainerView.Dispose ();
                ContainerView = null;
            }

            if (NameLabel != null) {
                NameLabel.Dispose ();
                NameLabel = null;
            }

            if (NameLabelHeightConstraint != null) {
                NameLabelHeightConstraint.Dispose ();
                NameLabelHeightConstraint = null;
            }
        }
    }
}