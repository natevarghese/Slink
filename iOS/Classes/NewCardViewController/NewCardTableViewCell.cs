using System;

using Foundation;
using UIKit;

namespace Slink.iOS
{
    public partial class NewCardTableViewCell : BaseTableViewCell
    {
        public static readonly NSString Key = new NSString("NewCardTableViewCell");
        public static readonly UINib Nib;
        public static readonly nfloat DefaultHeight = 44;

        protected NewCardTableViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static NewCardTableViewCell Create()
        {
            return UINib.FromName("NewCardTableViewCell", NSBundle.MainBundle).Instantiate(null, null)[0] as NewCardTableViewCell;
        }


        public override void Reset()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;

            Label.Text = null;
            RightView.BackgroundColor = UIColor.Clear;
        }

        public void BindDataToView(NewCardModel model)
        {
            Reset();

            Label.Text = model.Title;
            RightView.BackgroundColor = ColorUtils.FromHexString(model.ColorHexString, (model.IsTitle) ? UIColor.Clear : UIColor.White);
            RightViewSuperview.BackgroundColor = model.IsTitle ? UIColor.Clear : UIColor.White;
        }
    }
}
