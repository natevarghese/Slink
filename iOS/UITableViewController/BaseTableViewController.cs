using System;
using System.Threading.Tasks;
using UIKit;
using Foundation;
using CoreGraphics;
using System.Threading;

namespace Slink.iOS
{
    public abstract class BaseTableViewController<T> : UITableViewController
    {
        public UIView TableViewLoadingBackground, TableViewEmptyBackground;
        public bool PullToRefresh = true, FetchExecuting;
        public BaseTableViewSource<T> TableSource;

        protected CancellationTokenSource CancelToken;

        NSObject ObserveWillEnterForeground;


        public BaseTableViewController(UITableViewStyle style) : base(style) { }
        public BaseTableViewController(IntPtr handle) : base(handle) { }
        public BaseTableViewController(string xibName, NSBundle bundle) : base(xibName, bundle) { }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.Clear;

            AutomaticallyAdjustsScrollViewInsets = false;
            View.TranslatesAutoresizingMaskIntoConstraints = false;

            TableView.TableHeaderView = new UIView(new CGRect(0, 0, TableView.Frame.Size.Width, 0.1f));
            TableView.TableFooterView = new UIView(new CGRect(0, 0, TableView.Frame.Size.Width, 0.1f));
            TableView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.Interactive;
            TableView.RowHeight = UITableView.AutomaticDimension;

            ObserveWillEnterForeground = UIApplication.Notifications.ObserveWillEnterForeground((sender, args) =>
            {
                ViewWillAppear(true);
            });

            if (PullToRefresh)
            {
                RefreshControl = new UIRefreshControl();
                RefreshControl.TintColor = UIColor.White;
                RefreshControl.ValueChanged += (sender, e) =>
                {
                    FetchTableDataFromServer();
                };
            }

            var ai = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);
            ai.StartAnimating();
            TableViewLoadingBackground = ai;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            CancelToken = new CancellationTokenSource();

            ResetTableViewController();
            if (TableViewLoadingBackground != null && TableSource.IsEmpty())
            {
                PlaceBackgroundView(TableViewLoadingBackground);
            }

            FetchTableDataFromDatabase();
        }
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            ObserveWillEnterForeground?.Dispose();
            ObserveWillEnterForeground = null;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            CancelToken?.Cancel();
            CancelToken?.Dispose();
            CancelToken = null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                TableViewLoadingBackground?.Dispose();
                TableViewLoadingBackground = null;

                TableViewEmptyBackground?.Dispose();
                TableViewEmptyBackground = null;
            }
        }

        public void IfEmpty(bool endOfSequence)
        {

            RefreshControl?.EndRefreshing();

            if (endOfSequence || !TableSource.IsEmpty())
            {
                RemoveAnyExistingTableViewBackground();

                if (TableSource.IsEmpty())
                {
                    if (TableViewEmptyBackground != null)
                    {
                        PlaceBackgroundView(TableViewEmptyBackground);
                    }
                }

            }
        }

        private void RemoveAnyExistingTableViewBackground()
        {
            if (TableView.BackgroundView != null)
                TableView.BackgroundView = new UIView();
        }

        private void PlaceBackgroundView(UIView view)
        {
            if (view == null) return;

            RemoveAnyExistingTableViewBackground();

            TableView.BackgroundView = view;
        }

        public virtual void BindHeaderAndFooter(BaseHeaderFooterViewController header, BaseHeaderFooterViewController footer)
        {
            TableSource.HeaderViewController = header;
            TableSource.FooterViewController = footer;
        }

        public virtual void ResetTableViewController() { }
        public virtual void FetchTableDataFromDatabase() { }
        public virtual Task FetchTableDataFromServer() { return Task.CompletedTask; }
    }
}

