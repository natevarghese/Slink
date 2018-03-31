using System;

using Foundation;
using UIKit;

namespace Slink.iOS
{
    public partial class SettingsTableViewCell : BaseTableViewCell
    {
        public static readonly NSString Key = new NSString("SettingsTableViewCell");
        public static readonly UINib Nib;

        protected SettingsTableViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static SettingsTableViewCell Create()
        {
            return UINib.FromName("SettingsTableViewCell", NSBundle.MainBundle).Instantiate(null, null)[0] as SettingsTableViewCell;
        }

        public override void Reset()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;

            Label.Text = null;
            RightLabel.Text = null;
        }

        public void BindDataToView(SettingsShared.SettingsModel model)
        {
            Reset();

            Label.Text = model.Title;
            RightLabel.Text = model.Value;
        }
    }
}
