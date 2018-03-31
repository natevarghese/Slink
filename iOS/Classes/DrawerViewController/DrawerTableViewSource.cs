using System;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public class DrawerTableViewSource : BaseTableViewSource<DrawerShared.DrawerModel>
    {
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(DrawerTableViewCell.Key) as DrawerTableViewCell;
            DrawerShared.DrawerModel item = TableItems[indexPath.Row];

            if (cell == null)
                cell = DrawerTableViewCell.Create();

            cell.BindDataToView(item);

            return cell;
        }
    }
}
