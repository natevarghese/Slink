using System;
using Foundation;
using UIKit;
using CoreGraphics;
using Facebook.CoreKit;
using Facebook.LoginKit;

namespace Slink.iOS
{
    public class SettingsTableViewController : BaseTableViewController<SettingsShared.SettingsModel>
    {
        public SettingsShared Shared = new SettingsShared();

        public SettingsTableViewController() : base(UITableViewStyle.Plain) { }

        public override void ViewDidLoad()
        {
            PullToRefresh = false;

            base.ViewDidLoad();

            AutomaticallyAdjustsScrollViewInsets = false;

            TableSource = new SettingsTableViewSource();
            TableSource.ItemSelected += (NSIndexPath arg1, SettingsShared.SettingsModel arg2) =>
            {
                if (arg2 == null || String.IsNullOrEmpty(arg2.Title)) return;

                if (arg2.Title.Equals(SettingsShared.navigation_item_my_outlets, StringComparison.InvariantCultureIgnoreCase))
                {
                    //this is used to counter the auto reload. See NewCardTableViewController
                    Transporter.SharedInstance.RemoveObject(Transporter.NewOutletAddedTransporterKey);
                    Transporter.SharedInstance.RemoveObject(Transporter.NewOutletAddedValueTransporterKey);

                    ApplicationExtensions.PushViewController(new MyOutletsViewController(), true);
                    return;
                }

                if (arg2.Title.Equals(SettingsShared.navigation_item_edit_profile, StringComparison.InvariantCultureIgnoreCase))
                {
                    ApplicationExtensions.PushViewController(new EditProfileViewController(), true);
                    return;
                }


                if (arg2.Title.Equals(SettingsShared.navigation_item_design, StringComparison.InvariantCultureIgnoreCase))
                {
                    Shared.DesignChanged();
                    TableView.ReloadRows(new NSIndexPath[] { arg1 }, UITableViewRowAnimation.Automatic);
                    return;
                }

                if (arg2.Title.Equals(SettingsShared.navigation_item_logout, StringComparison.InvariantCultureIgnoreCase))
                {
                    RealmUserServices.Logout();

                    if (Profile.CurrentProfile != null)
                        new LoginManager().LogOut();

                    var iPersistant = ServiceLocator.Instance.Resolve<IPersistantStorage>();
                    iPersistant.RemoveAll();

                    ApplicationExtensions.LoadStoryboardRoot("Landing", false);
                    return;
                }
            };
            TableView.Source = TableSource;
            TableSource.SetItems(TableView, Shared.GetTableItems());
        }

    }
}
