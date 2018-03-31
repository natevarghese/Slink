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
    [Register ("FlyingObjectsContainterViewController")]
    partial class FlyingObjectsContainterViewController
    {
        [Outlet]
        UIKit.UIView AdViewSuperView { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint AdViewSuperViewHeightConstaint { get; set; }


        [Outlet]
        UIKit.UIView ContainerView { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint ContainerViewLeftMarginConstraint { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint ContainerViewRightMarginConstraint { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint ContainerViewTopMarginConstraint { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AdViewSuperView != null) {
                AdViewSuperView.Dispose ();
                AdViewSuperView = null;
            }

            if (AdViewSuperViewHeightConstaint != null) {
                AdViewSuperViewHeightConstaint.Dispose ();
                AdViewSuperViewHeightConstaint = null;
            }

            if (ContainerView != null) {
                ContainerView.Dispose ();
                ContainerView = null;
            }

            if (ContainerViewLeftMarginConstraint != null) {
                ContainerViewLeftMarginConstraint.Dispose ();
                ContainerViewLeftMarginConstraint = null;
            }

            if (ContainerViewRightMarginConstraint != null) {
                ContainerViewRightMarginConstraint.Dispose ();
                ContainerViewRightMarginConstraint = null;
            }

            if (ContainerViewTopMarginConstraint != null) {
                ContainerViewTopMarginConstraint.Dispose ();
                ContainerViewTopMarginConstraint = null;
            }
        }
    }
}