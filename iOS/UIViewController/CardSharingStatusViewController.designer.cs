// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Slink.iOS
{
    [Register("CardSharingStatusViewController")]
    partial class CardSharingStatusViewController
    {
        [Outlet]
        UIKit.UIButton PhoneButton { get; set; }

        [Outlet]
        UIKit.UIButton SharingButton { get; set; }

        [Action("BackgroundClicked:")]
        partial void BackgroundClicked(Foundation.NSObject sender);

        [Action("ChangeSharingStatusButtonClicked:")]
        partial void ChangeSharingStatusButtonClicked(Foundation.NSObject sender);

        [Action("SharingButtonClicked:")]
        partial void SharingButtonClicked(Foundation.NSObject sender);

        void ReleaseDesignerOutlets()
        {
            if (PhoneButton != null)
            {
                PhoneButton.Dispose();
                PhoneButton = null;
            }

            if (SharingButton != null)
            {
                SharingButton.Dispose();
                SharingButton = null;
            }
        }
    }
}
