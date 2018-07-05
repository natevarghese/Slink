// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Slink.iOS
{
    [Register("ColorPickerCollectionViewCell")]
    partial class ColorPickerCollectionViewCell
    {
        [Outlet]
        UIKit.UIView MyContentView { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (MyContentView != null)
            {
                MyContentView.Dispose();
                MyContentView = null;
            }
        }
    }
}
