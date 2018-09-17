using System;
using UIKit;

namespace Slink.iOS
{
    public class SharingViewController : ContainerViewController
    {
        public SharingViewController() : base()
        {
            TargetViewController = new SharingTableViewController();
        }

        public override void ViewDidLoad()
        {
            NetworkListenerEnabled = false;

            base.ViewDidLoad();

            if (((SharingTableViewController)TargetViewController).Shared.SelectedCard.IsMine())
            {
                NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Edit, (sender, e) =>
                {
                    //this is used to counter the auto reload. See NewCardTableViewController
                    Transporter.SharedInstance.RemoveObject(Transporter.NewOutletAddedTransporterKey);
                    Transporter.SharedInstance.RemoveObject(Transporter.NewOutletAddedValueTransporterKey);

                    var vc = new NewCardViewController();
                    (vc.TargetViewController as NewCardTableViewController).Shared.SelectedCard = (TargetViewController as SharingTableViewController).Shared.SelectedCard;
                    ApplicationExtensions.PushViewController(vc, true);
                });
            }
            else
            {
                NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Trash, (sender, e) =>
                {
                    ((SharingTableViewController)TargetViewController).Shared.DeleteCard();
                    NavigationController.PopViewController(true);
                });
            }

            RemoveBackBarButtonTitle();
        }
    }
}