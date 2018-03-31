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
    [Register ("HelloViewController")]
    partial class HelloViewController
    {
        [Outlet]
        Slink.iOS.LandingTextField FirstNameTextField { get; set; }


        [Outlet]
        Slink.iOS.LandingTextField LastNameTextField { get; set; }


        [Outlet]
        TPKeyboardAvoiding.TPKeyboardAvoidingScrollView ScrollView { get; set; }


        [Outlet]
        UIKit.UIButton StartButton { get; set; }


        [Action ("StartButtonClicked:")]
        partial void StartButtonClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (FirstNameTextField != null) {
                FirstNameTextField.Dispose ();
                FirstNameTextField = null;
            }

            if (LastNameTextField != null) {
                LastNameTextField.Dispose ();
                LastNameTextField = null;
            }

            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
            }

            if (StartButton != null) {
                StartButton.Dispose ();
                StartButton = null;
            }
        }
    }
}