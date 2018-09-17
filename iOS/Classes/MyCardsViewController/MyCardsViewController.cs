using System;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public partial class MyCardsViewController : BaseViewController
    {
        MyCardsTableViewController TableViewController = new MyCardsTableViewController();

        public MyCardsViewController() : base("MyCardsViewController") { }


        public override string Title
        {
            get
            {
                return DrawerShared.navigation_item_my_cards;
            }
            set { base.Title = value; }
        }

        public override void ViewDidLoad()
        {
            NetworkListenerEnabled = false;

            base.ViewDidLoad();

            CollectionView.RegisterClassForCell(typeof(MyCardsCollectionViewCell), MyCardsCollectionViewCell.Key);
            CollectionView.WeakDataSource = new MyCardsCollectionViewDataSource();
            CollectionView.WeakDelegate = new MyCardsCollectionViewDelegateFlowLayout();

            AddChildViewController(TableViewController);
            TableContainerView.AddSubview(TableViewController.View);
            TableContainerView.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, TableContainerView, NSLayoutAttribute.Top, 1, 0));
            TableContainerView.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Right, NSLayoutRelation.Equal, TableContainerView, NSLayoutAttribute.Right, 1, 0));
            TableContainerView.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, TableContainerView, NSLayoutAttribute.Bottom, 1, 0));
            TableContainerView.AddConstraint(NSLayoutConstraint.Create(TableViewController.View, NSLayoutAttribute.Left, NSLayoutRelation.Equal, TableContainerView, NSLayoutAttribute.Left, 1, 0));

            ToggleCollectionViewVisibility(false, false);
        }


        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }


        void ToggleCollectionViewVisibility(bool visible, bool animated)
        {
            if (animated)
            {
                View.LayoutIfNeeded();
                CollectionViewHeightConstraint.Constant = (visible) ? 75 : 0;

                UIView.Animate(.25, () =>
                {
                    CollectionView.Alpha = (visible) ? 1 : 0;
                    View.LayoutIfNeeded();
                }, () =>
                {
                    CollectionView.Hidden = (visible) ? false : true;
                });
            }
            else
            {
                CollectionView.Hidden = (visible) ? false : true;
                CollectionViewHeightConstraint.Constant = (visible) ? 75 : 0;
            }
        }
    }


    #region UICollectionViewDataSource
    class MyCardsCollectionViewDataSource : UICollectionViewDataSource
    {

        Random Random = new Random();
        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return 10;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (MyCardsCollectionViewCell)collectionView.DequeueReusableCell(MyCardsCollectionViewCell.Key, indexPath);
            var percentage = (nfloat)(Random.Next(100)) / 100;

            cell.UpdatePercentage(percentage);
            return cell;
        }
    }
    #endregion

    #region UICollectionViewDelegateFlowLayout
    public class MyCardsCollectionViewDelegateFlowLayout : UICollectionViewDelegateFlowLayout
    {
        public Action<UICollectionView, NSIndexPath> ItemClicked;

        public override UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            return new UIEdgeInsets(0, 8, 0, 0);
        }
        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ItemClicked?.Invoke(collectionView, indexPath);
        }
    }
    #endregion
}

