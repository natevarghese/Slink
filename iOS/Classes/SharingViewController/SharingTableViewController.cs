using System;
using Foundation;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using System.Linq;

namespace Slink.iOS
{
    public class SharingTableViewController : BaseTableViewController<Outlet>
    {
        public SharingShared Shared = new SharingShared();
        CardViewController HeaderView;

        public SharingTableViewController() : base("BaseTableViewController", null) { }

        public override void ViewDidLoad()
        {
            PullToRefresh = false;

            base.ViewDidLoad();

            TableSource = new SharingTableViewSource();
            ((SharingTableViewSource)TableSource).ShowsAccessory = Shared.SelectedCard.IsMine();
            TableSource.ItemSelected += (arg1, arg2) =>
            {
                if (Shared.SelectedCard.IsMine())
                {
                    var footer = ChildViewControllers.Where(c => c.GetType() == typeof(CardSharingStatusViewController)).FirstOrDefault() as CardSharingStatusViewController;
                    if (footer == null) return;
                    if (footer.Sharing) return;

                    Shared.OutletSelected(arg2);
                    TableView.ReloadRows(new NSIndexPath[] { arg1 }, UITableViewRowAnimation.Automatic);

                    HeaderView.Update(false);

                    footer.Update();
                    return;
                }

                ApplicationExtensions.OpenApplicationFromOutlet(arg2);

            };
            TableView.Source = TableSource;

            HeaderView = new CardViewController();
            HeaderView.SelectedCard = Shared.SelectedCard;
            HeaderView.View.Frame = new CGRect(0, 0, TableView.Frame.Width, CardViewController.GetCalculatedHeight());
            TableView.TableHeaderView = HeaderView.View;
            AddChildViewController(HeaderView);
            HeaderView.DidMoveToParentViewController(this);

            if (Shared.SelectedCard.IsMine())
            {
                CardSharingStatusViewController footerView = new CardSharingStatusViewController();
                footerView.SelectedCard = Shared.SelectedCard;
                footerView.View.Frame = new CGRect(0, 0, TableView.Frame.Width, footerView.GetHeight());
                TableView.TableFooterView = footerView.View;
                AddChildViewController(footerView);
            }

        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            RealmServices.ShareAllOutlets(Shared.SelectedCard);
        }

        public override void FetchTableDataFromDatabase()
        {
            base.FetchTableDataFromDatabase();

            TableSource.SetItems(TableView, Shared.GetTableItems());
            IfEmpty(true);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                HeaderView?.Dispose();
                HeaderView = null;
            }
        }
    }
}
