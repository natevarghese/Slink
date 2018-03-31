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
    [Register ("WebsiteViewController")]
    partial class WebsiteViewController
    {
        [Outlet]
        UIKit.UIActivityIndicatorView ActivityIndicatorView { get; set; }


        [Outlet]
        UIKit.UIButton SaveButton { get; set; }


        [Outlet]
        TPKeyboardAvoiding.TPKeyboardAvoidingScrollView ScrollView { get; set; }


        [Outlet]
        Slink.iOS.LandingTextField TextField { get; set; }


        [Outlet]
        UIKit.UIWebView WebView { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint WebViewSuperViewWidthConstraint { get; set; }


        [Action ("SaveButtonClicked:")]
        partial void SaveButtonClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (ActivityIndicatorView != null) {
                ActivityIndicatorView.Dispose ();
                ActivityIndicatorView = null;
            }

            if (SaveButton != null) {
                SaveButton.Dispose ();
                SaveButton = null;
            }

            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
            }

            if (TextField != null) {
                TextField.Dispose ();
                TextField = null;
            }

            if (WebView != null) {
                WebView.Dispose ();
                WebView = null;
            }

            if (WebViewSuperViewWidthConstraint != null) {
                WebViewSuperViewWidthConstraint.Dispose ();
                WebViewSuperViewWidthConstraint = null;
            }
        }
    }
}