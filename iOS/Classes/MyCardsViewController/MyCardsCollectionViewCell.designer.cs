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
    [Register ("MyCardsCollectionViewCell")]
    partial class MyCardsCollectionViewCell
    {
        [Outlet]
        Slink.iOS.WebImage Image { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Image != null) {
                Image.Dispose ();
                Image = null;
            }
        }
    }
}