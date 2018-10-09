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
    [Register ("CardSharingStatusViewController")]
    partial class CardSharingStatusViewController
    {
        [Outlet]
        UIKit.UIButton PhoneButton { get; set; }

        [Outlet]
        UIKit.UIButton SharingButton { get; set; }

        [Action ("BackgroundClicked:")]
        partial void BackgroundClicked (Foundation.NSObject sender);

        [Action ("ChangeSharingStatusButtonClicked:")]
        partial void ChangeSharingStatusButtonClicked (Foundation.NSObject sender);

        [Action ("SharingButtonClicked:")]
        partial void SharingButtonClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (PhoneButton != null) {
                PhoneButton.Dispose ();
                PhoneButton = null;
            }

            if (SharingButton != null) {
                SharingButton.Dispose ();
                SharingButton = null;
            }
        }
    }
}