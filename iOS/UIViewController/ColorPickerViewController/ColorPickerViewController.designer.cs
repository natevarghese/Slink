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
	[Register ("ColorPickerViewController")]
	partial class ColorPickerViewController
	{
		[Outlet]
		UIKit.UICollectionView CollectionView { get; set; }

		[Outlet]
		UIKit.UIView CurrentColorView { get; set; }

		[Outlet]
		UIKit.UIView NewColorView { get; set; }

		[Outlet]
		UIKit.UIView NewCurrentSuperview { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CollectionView != null) {
				CollectionView.Dispose ();
				CollectionView = null;
			}

			if (CurrentColorView != null) {
				CurrentColorView.Dispose ();
				CurrentColorView = null;
			}

			if (NewColorView != null) {
				NewColorView.Dispose ();
				NewColorView = null;
			}

			if (NewCurrentSuperview != null) {
				NewCurrentSuperview.Dispose ();
				NewCurrentSuperview = null;
			}
		}
	}
}
