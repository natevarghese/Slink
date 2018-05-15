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
    [Register ("CardFront")]
    partial class CardFront
    {
        [Outlet]
        UIKit.UICollectionView CollectionView { get; set; }


        [Outlet]
        Slink.iOS.WebImageButton HeaderImageButton { get; set; }


        [Outlet]
        UIKit.UIButton NoOutletsButton { get; set; }


        [Outlet]
        UIKit.UITextField TitleTextField { get; set; }


        [Outlet]
        UIKit.UITextField UserDisplayNameTextField { get; set; }


        [Action ("HeaderImageButtonClicked:")]
        partial void HeaderImageButtonClicked (Slink.iOS.WebImageButton sender);


        [Action ("NoOutletsButtonClicked:")]
        partial void NoOutletsButtonClicked (Foundation.NSObject sender);


        [Action ("test:")]
        partial void test (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (CollectionView != null) {
                CollectionView.Dispose ();
                CollectionView = null;
            }

            if (HeaderImageButton != null) {
                HeaderImageButton.Dispose ();
                HeaderImageButton = null;
            }

            if (NoOutletsButton != null) {
                NoOutletsButton.Dispose ();
                NoOutletsButton = null;
            }

            if (TitleTextField != null) {
                TitleTextField.Dispose ();
                TitleTextField = null;
            }

            if (UserDisplayNameTextField != null) {
                UserDisplayNameTextField.Dispose ();
                UserDisplayNameTextField = null;
            }
        }
    }
}