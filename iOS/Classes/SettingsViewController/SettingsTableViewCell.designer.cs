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
    [Register ("SettingsTableViewCell")]
    partial class SettingsTableViewCell
    {
        [Outlet]
        UIKit.UILabel Label { get; set; }


        [Outlet]
        UIKit.UILabel RightLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Label != null) {
                Label.Dispose ();
                Label = null;
            }

            if (RightLabel != null) {
                RightLabel.Dispose ();
                RightLabel = null;
            }
        }
    }
}