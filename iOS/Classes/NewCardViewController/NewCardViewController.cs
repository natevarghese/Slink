using System;
using Foundation;
using UIKit;
using Plugin.DeviceInfo;
using Plugin.DeviceInfo.Abstractions;

namespace Slink.iOS
{
    public class NewCardViewController : ContainerViewController
    {
        public NewCardViewController() : base()
        {
            TargetViewController = new NewCardTableViewController();
        }
        public override void ViewDidLoad()
        {
            NetworkListenerEnabled = false;

            base.ViewDidLoad();


            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Save, (sender, e) =>
            {
                var card = GetCard();
                var name = card.Name;
                if (String.IsNullOrEmpty(name) || name.Equals(Strings.Basic.new_card, StringComparison.InvariantCultureIgnoreCase))
                {
                    ShowCardMissingNameAlert();
                    return;
                }

                if (card.Outlets.Count == 0)
                {
                    ShowCardMissingOutletsAlert();
                    return;
                }

                var me = RealmUserServices.GetMe(false);
                var realm = RealmManager.SharedInstance.GetRealm(null);
                realm.Write(() =>
                {
                    card.Owner = me;
                });

                View.EndEditing(true);
                NavigationController.PopViewController(true);
            });


            var topRightButtonTitle = (GetCard() == null) ? "Discard" : Strings.Basic.delete;
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(topRightButtonTitle, UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                DeleteCard();
                NavigationController.PopToRootViewController(true);
            });
        }
        void ShowCardMissingNameAlert()
        {
            var alertStyle = UIAlertControllerStyle.Alert;

            UIAlertController AlertController = UIAlertController.Create(Strings.Basic.error, Strings.Alerts.card_missing_name, alertStyle);
            AlertController.AddAction(UIAlertAction.Create(Strings.Basic.ok, UIAlertActionStyle.Default, (obj) =>
            {
                (TargetViewController as NewCardTableViewController).FocusOnName();
            }));
            AlertController.AddAction(UIAlertAction.Create(Strings.Basic.delete_card, UIAlertActionStyle.Destructive, (obj) =>
            {
                DeleteCard();
                NavigationController.PopToRootViewController(true);
            }));
            PresentViewController(AlertController, true, null);
        }
        void ShowCardMissingOutletsAlert()
        {
            var alertStyle = UIAlertControllerStyle.Alert;

            UIAlertController AlertController = UIAlertController.Create(Strings.Alerts.card_missing_outlets, null, alertStyle);
            AlertController.AddAction(UIAlertAction.Create(Strings.Basic.ok, UIAlertActionStyle.Default, (obj) =>
            {
                //scroll to footer
                (TargetViewController as NewCardTableViewController).TableView.ScrollRectToVisible((TargetViewController as NewCardTableViewController).TableView.TableFooterView.Frame, true);
            }));
            AlertController.AddAction(UIAlertAction.Create(Strings.Basic.delete_card, UIAlertActionStyle.Destructive, (obj) =>
            {
                View.EndEditing(true);
                NavigationController.PopViewController(true);
            }));
            PresentViewController(AlertController, true, null);
        }


        Card GetCard()
        {
            return (TargetViewController as NewCardTableViewController).Shared.SelectedCard;
        }
        void DeleteCard()
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                GetCard().Deleted = true;
            });
        }
    }
}
