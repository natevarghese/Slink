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
    [Register ("NewCardTableViewCell")]
    partial class NewCardTableViewCell
    {
        [Outlet]
        UIKit.UILabel Label { get; set; }


        [Outlet]
        UIKit.UIView RightView { get; set; }


        [Outlet]
        UIKit.UIView RightViewSuperview { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Label != null) {
                Label.Dispose ();
                Label = null;
            }

            if (RightView != null) {
                RightView.Dispose ();
                RightView = null;
            }

            if (RightViewSuperview != null) {
                RightViewSuperview.Dispose ();
                RightViewSuperview = null;
            }
        }
    }
}