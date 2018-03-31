using System;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public class SettingsTableViewSource : BaseTableViewSource<SettingsShared.SettingsModel>
    {
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(SettingsTableViewCell.Key) as SettingsTableViewCell;
            SettingsShared.SettingsModel item = TableItems[indexPath.Row];

            if (cell == null)
                cell = SettingsTableViewCell.Create();

            cell.BindDataToView(item);

            return cell;
        }
    }
}
