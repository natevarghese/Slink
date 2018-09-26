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
    [Register ("ColorPickerCollectionViewCell")]
    partial class ColorPickerCollectionViewCell
    {
        [Outlet]
        UIKit.UIView MyContentView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (MyContentView != null) {
                MyContentView.Dispose ();
                MyContentView = null;
            }
        }
    }
}