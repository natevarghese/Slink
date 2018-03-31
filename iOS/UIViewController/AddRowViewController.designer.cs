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
    [Register ("AddRowViewController")]
    partial class AddRowViewController
    {
        [Outlet]
        UIKit.UILabel Label { get; set; }


        [Action ("AddRowClicked:")]
        partial void AddRowClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (Label != null) {
                Label.Dispose ();
                Label = null;
            }
        }
    }
}