using System;
using Foundation;
using UIKit;
using CoreGraphics;

namespace Slink.iOS
{
    public class MyCardsTableViewController : BaseTableViewController<Card>
    {
        public MyCardsShared Shared = new MyCardsShared();
        NSObject CollectionViewTappedNotificaion;


        public MyCardsTableViewController() : base("BaseTableViewController", null) { }

        public override void ViewDidLoad()
        {
            PullToRefresh = false;

            base.ViewDidLoad();


            TableSource = new MyCardsTableViewSource();
            TableSource.ItemSelected += (NSIndexPath arg1, Card arg2) =>
            {
                var vc = new SharingViewController();
                (vc.TargetViewController as SharingTableViewController).Shared.SelectedCard = arg2;
                ApplicationExtensions.PushViewController(vc, true);
            };
            TableView.Source = TableSource;
            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            var label = LabelWithActivityIndicatorView.Create();
            label.BindDataToView("No Cards", true);
            TableViewEmptyBackground = label;

            AddRowViewController FooterView = new AddRowViewController();
            FooterView.LabelAddText = MyCardsShared.CreateNewCard;
            FooterView.Clicked += (editingCreateNewCard) =>
            {
                var vc = new NewCardViewController();
                (vc.TargetViewController as NewCardTableViewController).Shared.SelectedCard = Card.Create();
                ApplicationExtensions.PushViewController(vc, true);
            };
            FooterView.View.Frame = new CGRect(0, 0, TableView.Frame.Width, FooterView.GetHeight());
            TableView.TableFooterView = FooterView.View;
            AddChildViewController(FooterView);



            var swipeGesture = new UISwipeGestureRecognizer((obj) =>
            {
                var swipeLocatioin = obj.LocationInView(TableView);

                var indexPath = TableView.IndexPathForRowAtPoint(swipeLocatioin);
                if (indexPath == null) return;

                var cell = TableView.CellAt(indexPath) as MyCardsTableViewCell;
                if (cell == null) return;

                var model = TableSource.GetModelInList(indexPath);
                if (model == null) return;

                cell.Flip(model);
            });
            swipeGesture.Direction = UISwipeGestureRecognizerDirection.Left | UISwipeGestureRecognizerDirection.Right;
            TableView.AddGestureRecognizer(swipeGesture);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            CollectionViewTappedNotificaion = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(Strings.InternalNotifications.notification_collection_view_tapped), (obj) =>
            {
                InvokeOnMainThread(() =>
                {
                    var indexPath = obj.Object as NSIndexPath;
                    if (indexPath == null) return;

                    var card = TableSource.GetModelInList(indexPath);

                    var vc = new SharingViewController();
                    (vc.TargetViewController as SharingTableViewController).Shared.SelectedCard = card;
                    ApplicationExtensions.PushViewController(vc, true);
                });
            });
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            CollectionViewTappedNotificaion?.Dispose();
            CollectionViewTappedNotificaion = null;
        }

        public override void FetchTableDataFromDatabase()
        {
            base.FetchTableDataFromDatabase();

            TableSource.SetItems(TableView, Shared.GetMyCards(true));
            IfEmpty(true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                CollectionViewTappedNotificaion?.Dispose();
                CollectionViewTappedNotificaion = null;
            }
        }
    }
}
