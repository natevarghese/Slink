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
    [Register ("EditProfileViewController")]
    partial class EditProfileViewController
    {
        [Outlet]
        Slink.iOS.LandingTextField FirstNameTextField { get; set; }


        [Outlet]
        Slink.iOS.LandingTextField LastNameTextField { get; set; }


        [Outlet]
        Slink.iOS.WebImageButton ProfileImageButton { get; set; }


        [Outlet]
        UIKit.UIButton SaveButton { get; set; }


        [Outlet]
        TPKeyboardAvoiding.TPKeyboardAvoidingScrollView ScrollView { get; set; }


        [Action ("ProfileImageButtonClicked:")]
        partial void ProfileImageButtonClicked (Foundation.NSObject sender);


        [Action ("SaveButtonClicked:")]
        partial void SaveButtonClicked (Foundation.NSObject sender);

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

            if (ProfileImageButton != null) {
                ProfileImageButton.Dispose ();
                ProfileImageButton = null;
            }

            if (SaveButton != null) {
                SaveButton.Dispose ();
                SaveButton = null;
            }

            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
            }
        }
    }
}