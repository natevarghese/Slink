using System;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public class SharingTableViewSource : BaseTableViewSource<Outlet>
    {
        public bool ShowsAccessory;
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(DrawerTableViewCell.Key) as DrawerTableViewCell;
            Outlet item = TableItems[indexPath.Row];

            if (cell == null)
                cell = DrawerTableViewCell.Create();

            cell.BindDataToView(item, ShowsAccessory);

            return cell;
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return false;
        }
    }
}
