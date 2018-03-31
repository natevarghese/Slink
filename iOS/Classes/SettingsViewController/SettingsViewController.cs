using System;
using UIKit;

namespace Slink.iOS
{
    public partial class SettingsViewController : BaseViewController
    {
        SettingsTableViewController TableViewController = new SettingsTableViewController();

        public SettingsViewController() : base("SettingsViewController") { }


        public override string Title
        {
            get
            {
                return DrawerShared.navigation_item_settings;
            }
            set { base.Title = value; }
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AutomaticallyAdjustsScrollViewInsets = false;
            TableViewController.AutomaticallyAdjustsScrollViewInsets = false;

            TableViewController.View.TranslatesAutoresizingMaskIntoConstraints = false;
            AddChildViewController(TableViewController);
            ContainerView.AddSubview(TableViewController.View);
            ContainerView.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Top, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Right, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Bottom, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Left, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Left, 1, 0));

            RemoveBackBarButtonTitle();
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