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
    [Register ("MyCardsViewController")]
    partial class MyCardsViewController
    {
        [Outlet]
        UIKit.UICollectionView CollectionView { get; set; }

        [Outlet]
        UIKit.NSLayoutConstraint CollectionViewHeightConstraint { get; set; }

        [Outlet]
        UIKit.UIView TableContainerView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CollectionView != null) {
                CollectionView.Dispose ();
                CollectionView = null;
            }

            if (CollectionViewHeightConstraint != null) {
                CollectionViewHeightConstraint.Dispose ();
                CollectionViewHeightConstraint = null;
            }

            if (TableContainerView != null) {
                TableContainerView.Dispose ();
                TableContainerView = null;
            }
        }
    }
}