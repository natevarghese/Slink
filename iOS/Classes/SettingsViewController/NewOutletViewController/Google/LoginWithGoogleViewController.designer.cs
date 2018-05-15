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
    [Register ("LoginWithGoogleViewController")]
    partial class LoginWithGoogleViewController
    {
        [Outlet]
        UIKit.UILabel GoogleNameLabel { get; set; }


        [Outlet]
        Slink.iOS.WebImage ImageView { get; set; }


        [Outlet]
        TPKeyboardAvoiding.TPKeyboardAvoidingScrollView ScrollView { get; set; }


        [Action ("GoogleSignInButtonClicked:")]
        partial void GoogleSignInButtonClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (GoogleNameLabel != null) {
                GoogleNameLabel.Dispose ();
                GoogleNameLabel = null;
            }

            if (ImageView != null) {
                ImageView.Dispose ();
                ImageView = null;
            }

            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
            }
        }
    }
}