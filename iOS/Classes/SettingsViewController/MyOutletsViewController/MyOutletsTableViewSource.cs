using System;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public class MyOutletsTableViewSource : BaseTableViewSource<Outlet>
    {
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(DrawerTableViewCell.Key) as DrawerTableViewCell;
            if (cell == null)
                cell = DrawerTableViewCell.Create();

            var item = TableItems[indexPath.Row];
            cell.BindDataToView(item, false);
            return cell;
        }
        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = TableItems[indexPath.Row];
            return !item.Locked;
        }
    }
}
