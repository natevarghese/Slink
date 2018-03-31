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
    [Register ("LoginWithFacebookViewController")]
    partial class LoginWithFacebookViewController
    {
        [Outlet]
        UIKit.UIView FacebookButtonSuperview { get; set; }


        [Outlet]
        UIKit.UILabel FacebookNameLabel { get; set; }


        [Outlet]
        UIKit.UIView FacebookProfilePictureSuperview { get; set; }


        [Outlet]
        TPKeyboardAvoiding.TPKeyboardAvoidingScrollView ScrollView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (FacebookButtonSuperview != null) {
                FacebookButtonSuperview.Dispose ();
                FacebookButtonSuperview = null;
            }

            if (FacebookNameLabel != null) {
                FacebookNameLabel.Dispose ();
                FacebookNameLabel = null;
            }

            if (FacebookProfilePictureSuperview != null) {
                FacebookProfilePictureSuperview.Dispose ();
                FacebookProfilePictureSuperview = null;
            }

            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
            }
        }
    }
}