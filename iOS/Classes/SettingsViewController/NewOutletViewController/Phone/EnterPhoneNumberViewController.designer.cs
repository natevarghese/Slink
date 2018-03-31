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
    [Register ("EnterPhoneNumberViewController")]
    partial class EnterPhoneNumberViewController
    {
        [Outlet]
        Slink.iOS.LandingTextField PhoneNumberTextField { get; set; }

        [Outlet]
        TPKeyboardAvoiding.TPKeyboardAvoidingScrollView ScrollView { get; set; }

        [Outlet]
        UIKit.UIButton ValidateButton { get; set; }

        [Action ("ValidateButtonClicked:")]
        partial void ValidateButtonClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (PhoneNumberTextField != null) {
                PhoneNumberTextField.Dispose ();
                PhoneNumberTextField = null;
            }

            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
            }

            if (ValidateButton != null) {
                ValidateButton.Dispose ();
                ValidateButton = null;
            }
        }
    }
}