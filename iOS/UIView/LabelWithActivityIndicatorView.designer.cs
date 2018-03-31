// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Slink.iOS
{
    [Register ("LabelWithActivityIndicatorView")]
    partial class LabelWithActivityIndicatorView
    {
        [Outlet]
        UIKit.UIActivityIndicatorView ActivityIndicatorView { get; set; }


        [Outlet]
        UIKit.UILabel Label { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint LabelTrailingSpaceConstraint { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ActivityIndicatorView != null) {
                ActivityIndicatorView.Dispose ();
                ActivityIndicatorView = null;
            }

            if (Label != null) {
                Label.Dispose ();
                Label = null;
            }

            if (LabelTrailingSpaceConstraint != null) {
                LabelTrailingSpaceConstraint.Dispose ();
                LabelTrailingSpaceConstraint = null;
            }
        }
    }
}