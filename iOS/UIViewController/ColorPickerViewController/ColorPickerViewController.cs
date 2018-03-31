using System;
using CoreGraphics;
using CoreImage;
using Foundation;
using UIKit;
using System.Collections.Generic;


namespace Slink.iOS
{
    public partial class ColorPickerViewController : UIViewController
    {
        public Action<UIColor> ColorPicked;
        public string LabelTitle;
        public UIColor StartingColor, SelectedColor;
        public nfloat DefaultOpacityAsDecimalOfPercentage = 0.3f;

        ColorPickerShared Shared = new ColorPickerShared();

        public ColorPickerViewController() : base("ColorPickerViewController", null) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = LabelTitle;

            CurrentColorView.BackgroundColor = StartingColor;
            SelectedColor = UIColor.Clear;
            if (StartingColor != null)
            {
                SelectedColor = StartingColor;
            }
            UpdateCurrentColor();

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Save, (sender, e) =>
            {
                ColorPicked?.Invoke(SelectedColor);
                DismissViewController(true, null);
            });

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Stop, (sender, e) =>
            {
                DismissViewController(true, null);
            });

            CollectionView.RegisterNibForCell(UINib.FromName(ColorPickerCollectionViewCell.Identifier, NSBundle.MainBundle), ColorPickerCollectionViewCell.Identifier);
            var collectionViewFlowDelegate = new ColorPIckerCollectionViewFlowDelegate();
            collectionViewFlowDelegate.ItemClicked += (NSIndexPath arg1) =>
            {
                var model = Shared.GetCollectionItems()[arg1.Row];

                var color = UIColor.FromRGBA(model.color[0], model.color[1], model.color[2], model.color[3]);
                SelectedColor = color;
                UpdateCurrentColor();
            };
            CollectionView.Delegate = collectionViewFlowDelegate;

            var collectionViewDataSource = new ColorPickerCollectionViewDataSource();
            collectionViewDataSource.SetCollectionItems(CollectionView, Shared.GetCollectionItems());
            CollectionView.DataSource = collectionViewDataSource;

            //required to make the scrollview appear under the cells
            CollectionView.ContentInset = new UIEdgeInsets(0, 0, 10, 0);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NewCurrentSuperview.Alpha = 0;
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            UIView.Animate(0.5, () =>
            {
                NewCurrentSuperview.Alpha = 1;
            });

            CollectionView.FlashScrollIndicators();

        }

        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            coordinator.AnimateAlongsideTransition((IUIViewControllerTransitionCoordinatorContext obj) =>
            {
                CollectionView.ReloadData();
            }, (IUIViewControllerTransitionCoordinatorContext obj) => { });

            base.ViewWillTransitionToSize(toSize, coordinator);
        }
        void UpdateCurrentColor()
        {
            NewColorView.BackgroundColor = SelectedColor;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                SelectedColor?.Dispose();
                SelectedColor = null;

                StartingColor?.Dispose();
                StartingColor = null;

                ColorPicked = null;
            }
        }
    }
}