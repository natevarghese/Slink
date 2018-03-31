using System;

using Foundation;
using UIKit;

namespace Slink.iOS
{
    public partial class DrawerTableViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("DrawerTableViewCell");
        public static readonly UINib Nib;

        protected DrawerTableViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static DrawerTableViewCell Create()
        {
            return UINib.FromName("DrawerTableViewCell", NSBundle.MainBundle).Instantiate(null, null)[0] as DrawerTableViewCell;
        }

        public void Reset()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;

            LeftLabel.Text = null;
            LeftImageView.Image = null;
            NotificaionView.Hidden = true;

            LeftImageViewWidthConstraint.Constant = 28;
        }

        public void BindDataToView(DrawerShared.DrawerModel model)
        {
            Reset();

            LeftLabel.Text = model.Title;
            LeftImageView.Image = UIImage.FromBundle(model.Image);

            if (model.Notifications > 0)
            {
                NotificaionView.Layer.MasksToBounds = true;
                NotificaionView.Layer.CornerRadius = NotificaionView.Frame.Width / 2;
                NotificaionView.Hidden = false;
            }
        }
        public void BindDataToView(Outlet model, bool showsAccessory)
        {
            Reset();

            LeftLabel.Text = model.Name;
            LeftImageView.SetImage(model.RemoteURL, null, null, null);

            if (!showsAccessory)
                Accessory = UITableViewCellAccessory.None;
            else
                Accessory = !model.Omitted ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

        }
        public void BindDataToView(NewCardModel model)
        {
            Reset();

            LeftLabel.Text = model.Outlet.Name;
            LeftImageView.SetImage(model.Outlet.RemoteURL, null, null, null);
        }
    }
}
