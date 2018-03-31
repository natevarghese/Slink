using System;

using UIKit;

namespace Slink.iOS
{
    public partial class MyOutletsViewController : BaseViewController
    {
        MyOutletsTableViewController TableViewController = new MyOutletsTableViewController();

        public Action<Outlet> OutletSelected;

        public MyOutletsViewController() : base("MyOutletsViewController") { }


        public override string Title
        {
            get
            {
                return SettingsShared.navigation_item_my_outlets;
            }
            set { base.Title = value; }
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AutomaticallyAdjustsScrollViewInsets = false;
            TableViewController.AutomaticallyAdjustsScrollViewInsets = false;

            TableViewController.OutletSelected = OutletSelected;

            AddChildViewController(TableViewController);
            ContainerView.AddSubview(TableViewController.View);
            ContainerView.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Top, 1, 44));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Right, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Bottom, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Left, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Left, 1, 0));

            RemoveBackBarButtonTitle();
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Title = SettingsShared.navigation_item_my_outlets;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                TableViewController?.Dispose();
                TableViewController = null;
            }
        }
    }
}