using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CoreAnimation;

namespace Slink.iOS
{
    public class MyOutletsTableViewController : BaseTableViewController<Outlet>
    {
        public Action<Outlet> OutletSelected;

        public MyOutletsTableViewController() : base(UITableViewStyle.Plain) { }

        public override void ViewDidLoad()
        {
            PullToRefresh = false;

            base.ViewDidLoad();

            TableSource = new MyOutletsTableViewSource();
            TableSource.ItemSelected += (NSIndexPath arg1, Outlet arg2) =>
            {
                OutletSelected?.Invoke(arg2);
            };
            TableSource.RowDeleted += (NSIndexPath arg1, Outlet arg2) =>
            {
                RealmServices.DeleteOutlet(arg2);
                TableSource.TableItems = RealmServices.GetMyOutlets();
            };
            TableView.Source = TableSource;


            var label = LabelWithActivityIndicatorView.Create();
            label.BindDataToView("No Outlets", true);
            TableViewEmptyBackground = label;


            AddRowViewController FooterView = new AddRowViewController();
            FooterView.LabelAddText = Strings.TableViewFooters.table_view_footer_create_new_outlet;
            FooterView.Clicked += (editing) =>
            {
                CATransition transition = CATransition.CreateAnimation();
                transition.Duration = 0.3;
                transition.Type = CATransition.TransitionFade;
                NavigationController.View.Layer.AddAnimation(transition, null);
                NavigationController.PushViewController(new NewOutletViewController(), false);
            };
            FooterView.View.Frame = new CGRect(0, 0, TableView.Frame.Width, FooterView.GetHeight());
            TableView.TableFooterView = FooterView.View;
            AddChildViewController(FooterView);
        }

        public override void FetchTableDataFromDatabase()
        {
            base.FetchTableDataFromDatabase();

            TableSource.SetItems(TableView, RealmServices.GetMyOutlets());
            IfEmpty(true);
        }
    }
}