using System;

using Foundation;
using UIKit;

namespace Slink.iOS
{
    public partial class NewOutletCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("NewOutletCollectionViewCell");
        public static readonly UINib Nib;

        protected NewOutletCollectionViewCell(IntPtr handle) : base(handle) { }

        public void Reset()
        {
            Button.Enabled = true;
            Button.SetTitle(null, new UIControlState());
            Label.Text = null;
            Button.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            Button.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
        }

        public void BindDataToView(Outlet model)
        {
            Reset();

            Button.SetImage(model.RemoteURL, null, null);
            Button.Enabled = model.AvailbleForAddition;
            Button.VerticalAlignment = UIControlContentVerticalAlignment.Fill;
            Button.HorizontalAlignment = UIControlContentHorizontalAlignment.Fill;

            Label.Text = model.Type;
        }
    }
}
