using System;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public class DrawerTableViewController : BaseTableViewController<DrawerShared.DrawerModel>
    {
        public DrawerShared Shared = new DrawerShared();

        public DrawerTableViewController() : base("BaseTableViewController", null) { }

        public override void ViewDidLoad()
        {
            PullToRefresh = false;

            base.ViewDidLoad();

            TableSource = new DrawerTableViewSource();
            TableSource.ItemSelected += (NSIndexPath arg1, DrawerShared.DrawerModel arg2) =>
            {
                if (arg2 == null || String.IsNullOrEmpty(arg2.Title)) return;

                Type target = null;

                if (arg2.Title.Equals(DrawerShared.navigation_item_my_cards, StringComparison.InvariantCultureIgnoreCase))
                {
                    target = typeof(MyCardsViewController);
                }

                else if (arg2.Title.Equals(DrawerShared.navigation_item_settings, StringComparison.InvariantCultureIgnoreCase))
                {
                    target = typeof(SettingsViewController);
                }

                else if (arg2.Title.Equals(DrawerShared.navigation_item_discover, StringComparison.InvariantCultureIgnoreCase))
                {
                    target = typeof(DiscoverViewController);
                }

                else if (arg2.Title.Equals(DrawerShared.navigation_item_connections, StringComparison.InvariantCultureIgnoreCase))
                {
                    target = typeof(ConnectionsTableViewController);
                }

                if (target != null)
                {
                    ApplicationExtensions.ActivatePage(target, true, false);
                    DismissViewController(true, null);
                }
            };
            TableView.Source = TableSource;
        }
        public override void FetchTableDataFromDatabase()
        {
            base.FetchTableDataFromDatabase();
            TableSource.SetItems(TableView, Shared.GetTableItems());
            IfEmpty(true);
        }
    }
}
