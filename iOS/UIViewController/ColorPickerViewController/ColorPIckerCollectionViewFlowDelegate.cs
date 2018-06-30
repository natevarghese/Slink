using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;

namespace Slink.iOS
{
    public class ColorPIckerCollectionViewFlowDelegate : UICollectionViewDelegateFlowLayout
    {
        public Action<NSIndexPath> ItemClicked;

        nfloat MinLandscapeHeight = 32; //landscape on the se per row
        int MaxRows = 6;

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ItemClicked?.Invoke(indexPath);
        }

        public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            //smallest allowed size
            var minHeight = collectionView.Frame.Height / 2 - MaxRows;
            if (minHeight <= MinLandscapeHeight)
                return new CGSize(MinLandscapeHeight, MinLandscapeHeight);

            //needed for sizes where we can fit more than 1 row, but not 6 rows
            var notTheSmallestRowButStillNeedToBreakItDownHeight = Math.Floor((collectionView.Frame.Height - 10) / MinLandscapeHeight);
            if (notTheSmallestRowButStillNeedToBreakItDownHeight < MaxRows)
            {
                //todo move to new branch
                //whats the new size
                var adjustedSize = collectionView.Frame.Height / notTheSmallestRowButStillNeedToBreakItDownHeight;
                return new CGSize(adjustedSize, adjustedSize);
            }

            //caps at 6
            //var minheight = collectionView.Frame.Height / 7 - MaxRows;
            var collectionViewTopAndBottomInset = collectionView.ContentInset.Top + collectionView.ContentInset.Bottom;
            var totalCellTopAndBottomInset = 16 * MaxRows - 16;
            var minheight = (collectionView.Frame.Height - collectionViewTopAndBottomInset - totalCellTopAndBottomInset) / MaxRows;
            Console.WriteLine("minheight; " + minheight);
            return new CGSize(minheight, minheight);
        }

        public override void WillDisplayCell(UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
        {
            cell.Alpha = 0;
            UIView.Animate(0.5, () => { cell.Alpha = 1; });
        }
    }
}
