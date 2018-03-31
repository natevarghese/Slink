using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;
using System.Linq;

namespace Slink.iOS
{
    public partial class NewOutletViewController : BaseViewController
    {
        public NewOutletViewController() : base("NewOutletViewController") { }

        public override string Title
        {
            get
            {
                return "Select a type";
            }
            set { base.Title = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RemoveBackBarButtonTitle();

            CollectionView.RegisterNibForCell(UINib.FromName("NewOutletCollectionViewCell", NSBundle.MainBundle), NewOutletCollectionViewCell.Key);

            var dataSource = new NewOutletCollectionViewDataSource();
            dataSource.CollectionItems = RealmServices.GetAllAvailableOutlets();
            CollectionView.WeakDataSource = dataSource;

            var delegateFlowLayout = new NewOutletCollectionViewDelegateFlowLayout();
            delegateFlowLayout.ItemClicked += (UICollectionView arg1, NSIndexPath arg2) =>
            {
                var items = RealmServices.GetAllAvailableOutlets();
                var item = items[arg2.Row];
                if (item == null) return;
                if (!item.AvailbleForAddition)
                {
                    ShowDuplcateAccountAlert("For now, you can only have 1 " + item.Type + " account. We're working on it");
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_phone))
                {
                    NavigationController.PushViewController(new EnterPhoneNumberViewController(), true);
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_email))
                {

                    NavigationController.PushViewController(new EnterEmailAddressViewController(), true);
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_website))
                {
                    NavigationController.PushViewController(new WebsiteViewController(), true);
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_facebook))
                {
                    NavigationController.PushViewController(new LoginWithFacebookViewController(), true);
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_linkedIn))
                {
                    NavigationController.PushViewController(new LinkedInWebViewController(), true);
                    return;
                }


                if (item.Type.Equals(Outlet.outlet_type_github))
                {
                    NavigationController.PushViewController(new GithubWebViewController(), true);
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_instagram))
                {
                    NavigationController.PushViewController(new InstagramWebViewController(), true);
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_google))
                {
                    NavigationController.PushViewController(new LoginWithGoogleViewController(), true);
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_twitter))
                {
                    ShowTwitterAuthenticationFlow();
                    return;
                }

                if (item.Type.Equals(Outlet.outlet_type_pinterest))
                {
                    NavigationController.PushViewController(new PinterestWebViewController(), true);
                    return;
                }
            };
            CollectionView.WeakDelegate = delegateFlowLayout;

        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Title = "Select an Outlet";
        }
        void ShowDuplcateAccountAlert(string message)
        {
            UIAlertController AlertController = UIAlertController.Create(Strings.Basic.error, message, UIAlertControllerStyle.Alert);
            AlertController.AddAction(UIAlertAction.Create(Strings.Basic.ok, UIAlertActionStyle.Default, null));
            PresentViewController(AlertController, true, null);
        }

        void ShowTwitterAuthenticationFlow()
        {
            var auth = TwitterAuthenticator.GetTwitterAuthenticator((bool sucessful) =>
            {
                //dismisses twitter dialog
                DismissViewController(true, () =>
                {
                    if (sucessful)
                    {
                        var popToViewController = NavigationController.ViewControllers.Where(c => c.GetType() == typeof(MyOutletsViewController)).First();
                        NavigationController.PopToViewController(popToViewController, true);
                    }

                });
            });
            var ui = auth.GetUI();
            PresentViewController(ui, true, null);
        }
    }

    #region UICollectionViewDataSource
    class NewOutletCollectionViewDataSource : UICollectionViewDataSource
    {
        public List<Outlet> CollectionItems;

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return (CollectionItems == null) ? 0 : 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return (CollectionItems == null) ? 0 : CollectionItems.Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (NewOutletCollectionViewCell)collectionView.DequeueReusableCell(NewOutletCollectionViewCell.Key, indexPath);
            var item = CollectionItems[indexPath.Row];

            cell.BindDataToView(item);
            return cell;
        }
    }
    #endregion

    #region UICollectionViewDelegateFlowLayout
    public class NewOutletCollectionViewDelegateFlowLayout : UICollectionViewDelegateFlowLayout
    {
        public Action<UICollectionView, NSIndexPath> ItemClicked;
        public nfloat MaxWidth = 100;

        public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var width = collectionView.Frame.Size.Width / 3.0 - 8;
            return new CGSize(width, width);
        }
        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ItemClicked?.Invoke(collectionView, indexPath);
        }
    }
    #endregion
}

