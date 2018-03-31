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
    [Register ("DrawerViewController")]
    partial class DrawerViewController
    {
        [Outlet]
        UIKit.UIView ContainerView { get; set; }


        [Outlet]
        UIKit.UILabel FooterLabel { get; set; }


        [Outlet]
        UIKit.UILabel HandleLabel { get; set; }


        [Outlet]
        UIKit.UILabel InititalsLabel { get; set; }


        [Outlet]
        UIKit.UILabel NameLabel { get; set; }


        [Action ("HeaderViewClicked:")]
        partial void HeaderViewClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (ContainerView != null) {
                ContainerView.Dispose ();
                ContainerView = null;
            }

            if (FooterLabel != null) {
                FooterLabel.Dispose ();
                FooterLabel = null;
            }

            if (HandleLabel != null) {
                HandleLabel.Dispose ();
                HandleLabel = null;
            }

            if (InititalsLabel != null) {
                InititalsLabel.Dispose ();
                InititalsLabel = null;
            }

            if (NameLabel != null) {
                NameLabel.Dispose ();
                NameLabel = null;
            }
        }
    }
}