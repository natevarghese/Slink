using System;
using UIKit;

namespace Slink.iOS
{
    public abstract class BaseTableViewCell : UITableViewCell
    {
        public BaseTableViewCell(IntPtr handle) : base(handle)
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            LayoutIfNeeded();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            ContentView.SetNeedsLayout();
            ContentView.LayoutIfNeeded();

        }

        public abstract void Reset();
    }
}

