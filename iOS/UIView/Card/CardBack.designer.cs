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
    [Register ("CardBack")]
    partial class CardBack
    {
        [Outlet]
        UIKit.UITextField CompanyNameTextField { get; set; }


        [Outlet]
        Slink.iOS.WebImageButton LogoImageButton { get; set; }


        [Action ("LogoImageButtonClicked:")]
        partial void LogoImageButtonClicked (Slink.iOS.WebImageButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CompanyNameTextField != null) {
                CompanyNameTextField.Dispose ();
                CompanyNameTextField = null;
            }

            if (LogoImageButton != null) {
                LogoImageButton.Dispose ();
                LogoImageButton = null;
            }
        }
    }
}