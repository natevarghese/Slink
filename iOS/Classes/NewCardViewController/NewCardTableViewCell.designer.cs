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
	[Register ("NewCardTableViewCell")]
	partial class NewCardTableViewCell
	{
		[Outlet]
		UIKit.UILabel Label { get; set; }

		[Outlet]
		UIKit.UIView RightView { get; set; }

		[Outlet]
		UIKit.UIView RightViewSuperview { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Label != null) {
				Label.Dispose ();
				Label = null;
			}

			if (RightView != null) {
				RightView.Dispose ();
				RightView = null;
			}

			if (RightViewSuperview != null) {
				RightViewSuperview.Dispose ();
				RightViewSuperview = null;
			}
		}
	}
}
