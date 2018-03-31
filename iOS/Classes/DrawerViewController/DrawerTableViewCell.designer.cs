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
	[Register ("DrawerTableViewCell")]
	partial class DrawerTableViewCell
	{
		[Outlet]
		Slink.iOS.WebImage LeftImageView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint LeftImageViewWidthConstraint { get; set; }

		[Outlet]
		UIKit.UILabel LeftLabel { get; set; }

		[Outlet]
		UIKit.UILabel NotificaionView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LeftImageView != null) {
				LeftImageView.Dispose ();
				LeftImageView = null;
			}

			if (LeftLabel != null) {
				LeftLabel.Dispose ();
				LeftLabel = null;
			}

			if (NotificaionView != null) {
				NotificaionView.Dispose ();
				NotificaionView = null;
			}

			if (LeftImageViewWidthConstraint != null) {
				LeftImageViewWidthConstraint.Dispose ();
				LeftImageViewWidthConstraint = null;
			}
		}
	}
}
