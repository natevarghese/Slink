using Foundation;
using UIKit;

namespace Slink.iOS
{
    public class ConnectionsTableViewController : BaseTableViewController<SlinkUser>
    {
        public MyCardsShared Shared = new MyCardsShared();

        public ConnectionsTableViewController() : base(UITableViewStyle.Grouped) { }

        public override string Title
        {
            get
            {
                return DrawerShared.navigation_item_connections;
            }
            set { base.Title = value; }
        }

        public override void ViewDidLoad()
        {
            PullToRefresh = false;

            base.ViewDidLoad();

            TableSource = new ConnectionsTableViewSource();
            ((ConnectionsTableViewSource)TableSource).CardSelected += (NSIndexPath arg1, Card arg2) =>
            {
                var vc = new SharingViewController();
                vc.Title = arg2.Name;
                (vc.TargetViewController as SharingTableViewController).Shared.SelectedCard = arg2;
                ApplicationExtensions.PushViewController(vc, true);

            };
            TableView.Source = TableSource;
            TableView.SeparatorStyle = UIKit.UITableViewCellSeparatorStyle.None;


            var label = LabelWithActivityIndicatorView.Create();
            label.BindDataToView("No Connections", true);
            TableViewEmptyBackground = label;

            var swipeGesture = new UISwipeGestureRecognizer((obj) =>
            {
                var swipeLocatioin = obj.LocationInView(TableView);

                var indexPath = TableView.IndexPathForRowAtPoint(swipeLocatioin);
                if (indexPath == null) return;

                var cell = TableView.CellAt(indexPath) as MyCardsTableViewCell;
                if (cell == null) return;

                var model = ((ConnectionsTableViewSource)TableSource).GetCardAtIndexPath(indexPath);
                if (model == null) return;

                cell.Flip(model);
            });
            swipeGesture.Direction = UISwipeGestureRecognizerDirection.Left | UISwipeGestureRecognizerDirection.Right;
            TableView.AddGestureRecognizer(swipeGesture);
        }

        public override void FetchTableDataFromDatabase()
        {
            base.FetchTableDataFromDatabase();

            TableSource.SetItems(TableView, RealmServices.GetMyConnections());
            IfEmpty(true);
        }
    }
}