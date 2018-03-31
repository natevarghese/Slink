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
    [Register ("NewOutletCollectionViewCell")]
    partial class NewOutletCollectionViewCell
    {
        [Outlet]
        Slink.iOS.WebImageButton Button { get; set; }


        [Outlet]
        UIKit.UILabel Label { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Button != null) {
                Button.Dispose ();
                Button = null;
            }

            if (Label != null) {
                Label.Dispose ();
                Label = null;
            }
        }
    }
}