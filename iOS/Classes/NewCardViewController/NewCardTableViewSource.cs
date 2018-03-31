using System;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public class NewCardTableViewSource : BaseTableViewSource<NewCardModel>
    {
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {

            var item = TableItems[indexPath.Row];

            if (item.Editable)
            {
                var cell = tableView.DequeueReusableCell(NewCardEditableTableViewCell.Key) as NewCardEditableTableViewCell;
                if (cell == null)
                    cell = NewCardEditableTableViewCell.Create();

                cell.BindDataToView(item);
                return cell;
            }
            else if (item.Outlet != null)
            {
                var cell = tableView.DequeueReusableCell(DrawerTableViewCell.Key) as DrawerTableViewCell;
                if (cell == null)
                    cell = DrawerTableViewCell.Create();

                cell.BindDataToView(item);
                return cell;
            }
            else
            {
                var cell = tableView.DequeueReusableCell(NewCardTableViewCell.Key) as NewCardTableViewCell;
                if (cell == null)
                    cell = NewCardTableViewCell.Create();

                cell.BindDataToView(item);
                return cell;
            }
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = TableItems[indexPath.Row];
            if (item.IsTitle)
                return 100;

            return NewCardTableViewCell.DefaultHeight;
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = TableItems[indexPath.Row];
            return item.Outlet != null;
        }
    }
}
