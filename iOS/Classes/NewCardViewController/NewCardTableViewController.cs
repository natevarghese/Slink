using System;
using Foundation;
using UIKit;
using CoreGraphics;

namespace Slink.iOS
{
    public class NewCardTableViewController : BaseTableViewController<NewCardModel>
    {
        public NewCardShared Shared = new NewCardShared();
        CardViewController HeaderView = new CardViewController();
        NSObject TableViewCardEditingChangedNotification, NoOutletsTappedNotification;

        public NewCardTableViewController() : base("BaseTableViewController", null) { }

        public override void ViewDidLoad()
        {
            PullToRefresh = false;

            base.ViewDidLoad();

            TableSource = new NewCardTableViewSource();
            TableSource.ItemSelected += (NSIndexPath arg1, NewCardModel arg2) =>
            {
                if (arg2 == null || String.IsNullOrEmpty(arg2.Title)) return;

                var vc = new ColorPickerViewController();
                vc.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
                vc.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
                vc.LabelTitle = arg2.Title;
                vc.StartingColor = ColorUtils.FromHexString(arg2.ColorHexString, UIColor.White);
                vc.ColorPicked += (UIColor obj) =>
                {
                    if (obj == null && String.IsNullOrEmpty(arg2.Title)) return;

                    arg2.ColorHexString = ColorUtils.HexStringFromColor(obj);

                    if (arg2.Title.Equals(NewCardShared.new_card_model_border_color, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Shared.SelectedCard.UpdateStringProperty(() => Shared.SelectedCard.BorderColor, arg2.ColorHexString);
                        Shared.SelectedCard.ShowFront();
                    }

                    else if (arg2.Title.Equals(NewCardShared.new_card_model_background_color, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Shared.SelectedCard.UpdateStringProperty(() => Shared.SelectedCard.BackgroundColor, arg2.ColorHexString);
                        Shared.SelectedCard.ShowBack();
                    }

                    else if (arg2.Title.Equals(NewCardShared.new_card_model_company_name_text_color, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Shared.SelectedCard.UpdateStringProperty(() => Shared.SelectedCard.CompanyNameTextColor, arg2.ColorHexString);
                        Shared.SelectedCard.ShowBack();
                    }


                    HeaderView.Update(false);
                    TableView.ReloadRows(new NSIndexPath[] { arg1 }, UITableViewRowAnimation.Automatic);
                };

                var flyingObjectsContainerViewController = new FlyingObjectsContainterViewController();
                var clearNavigationController = flyingObjectsContainerViewController.ContainerNavigationController;
                clearNavigationController.SetViewControllers(new UIViewController[] { vc }, false);
                PresentViewController(flyingObjectsContainerViewController, false, null);

            };
            TableSource.RowDeleted += (NSIndexPath arg1, NewCardModel arg2) =>
            {
                if (arg2.Outlet == null) return;

                Shared.SelectedCard.RemoveOutlet(arg2.Outlet);
                TableSource.TableItems.Remove(arg2);
                HeaderView.Update(false);
            };
            TableView.Source = TableSource;

            HeaderView.Editable = true;
            HeaderView.SelectedCard = Shared.SelectedCard;
            HeaderView.View.Frame = new CGRect(0, 0, TableView.Frame.Width, CardViewController.GetCalculatedHeight());
            AddChildViewController(HeaderView);
            TableView.TableHeaderView = HeaderView.View;


            AddRowViewController FooterView = new AddRowViewController();
            FooterView.LabelAddText = NewCardShared.AddNewOutlet;
            FooterView.Clicked += (editing) =>
            {
                ShowMyOutletsViewController();

            };
            FooterView.View.Frame = new CGRect(0, 0, TableView.Frame.Width, FooterView.GetHeight());
            TableView.TableFooterView = FooterView.View;
            AddChildViewController(FooterView);

        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var shouldReload = false;
            if (Transporter.SharedInstance.ContainsKey(Transporter.NewOutletAddedTransporterKey))
                shouldReload = (bool)Transporter.SharedInstance.GetObjectForKey(Transporter.NewOutletAddedTransporterKey);

            if (shouldReload)
            {
                Outlet outlet = null;
                if (Transporter.SharedInstance.ContainsKey(Transporter.NewOutletAddedValueTransporterKey))
                    outlet = Transporter.SharedInstance.GetObjectForKey(Transporter.NewOutletAddedValueTransporterKey) as Outlet;

                if (outlet != null)
                {
                    AddOutletToCard(outlet, false);
                    HeaderView.Update(false);
                    FetchTableDataFromDatabase();

                    Transporter.SharedInstance.RemoveObject(Transporter.NewOutletAddedTransporterKey);
                    Transporter.SharedInstance.RemoveObject(Transporter.NewOutletAddedValueTransporterKey);
                }
            }
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            TableViewCardEditingChangedNotification = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(Strings.InternalNotifications.notification_card_editing_changed), (obj) =>
            {
                InvokeOnMainThread(() =>
                    {
                        TableSource.SetItems(TableView, Shared.GetTableItems());
                    });
            });

            NoOutletsTappedNotification = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(Strings.InternalNotifications.notification_no_outlets_tapped), (obj) =>
            {
                InvokeOnMainThread(() =>
                {
                    //scroll to footer
                    TableView.ScrollRectToVisible(TableView.TableFooterView.Frame, true);
                });
            });
        }
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            View.EndEditing(true);

            TableViewCardEditingChangedNotification?.Dispose();
            TableViewCardEditingChangedNotification = null;

            NoOutletsTappedNotification?.Dispose();
            NoOutletsTappedNotification = null;
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

                TableViewCardEditingChangedNotification?.Dispose();
                TableViewCardEditingChangedNotification = null;

                NoOutletsTappedNotification?.Dispose();
                NoOutletsTappedNotification = null;
            }
        }
        public void FocusOnName()
        {
            HeaderView.FocusOnName();
        }
        void ShowMyOutletsViewController()
        {
            Transporter.SharedInstance.SetObject(Transporter.NewOutletAddedTransporterKey, true);

            var vc = new MyOutletsViewController();
            vc.OutletSelected += (Outlet obj) =>
            {
                AddOutletToCard(obj, true);
            };
            ApplicationExtensions.PushViewController(vc, true);
        }
        void AddOutletToCard(Outlet obj, bool dismiss)
        {
            Shared.SelectedCard.AddOutlet(obj);
            FetchTableDataFromDatabase();
            HeaderView.Update(false);

            if (dismiss)
                NavigationController.PopViewController(true);
        }
    }
}
