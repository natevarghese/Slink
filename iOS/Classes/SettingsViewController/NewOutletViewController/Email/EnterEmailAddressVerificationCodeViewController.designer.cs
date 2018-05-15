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
    [Register ("EnterEmailAddressVerificationCodeViewController")]
    partial class EnterEmailAddressVerificationCodeViewController
    {
        [Outlet]
        UIKit.UIButton NextButton { get; set; }


        [Outlet]
        TPKeyboardAvoiding.TPKeyboardAvoidingScrollView ScrollView { get; set; }


        [Outlet]
        Slink.iOS.LandingTextField VerificationCodeTextField { get; set; }


        [Action ("NextButtonClicked:")]
        partial void NextButtonClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (NextButton != null) {
                NextButton.Dispose ();
                NextButton = null;
            }

            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
            }

            if (VerificationCodeTextField != null) {
                VerificationCodeTextField.Dispose ();
                VerificationCodeTextField = null;
            }
        }
    }
}