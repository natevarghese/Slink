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
    [Register ("LoginViewController")]
    partial class LoginViewController
    {
        [Outlet]
        UIKit.UIView FacebookButtonSuperview { get; set; }

        [Action ("CreateAccountButtonClicked:")]
        partial void CreateAccountButtonClicked (Foundation.NSObject sender);

        [Action ("ForgotPasswordButtonClicked:")]
        partial void ForgotPasswordButtonClicked (Foundation.NSObject sender);

        [Action ("LoginButtonClicked:")]
        partial void LoginButtonClicked (Foundation.NSObject sender);

        [Action ("TermsOfServiceClicked:")]
        partial void TermsOfServiceClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (FacebookButtonSuperview != null) {
                FacebookButtonSuperview.Dispose ();
                FacebookButtonSuperview = null;
            }
        }
    }
}