using System; using System.Collections.Generic; using Foundation; using UIKit; using CoreGraphics; namespace Slink.iOS {
    public class ColorPickerCollectionViewDataSource : UICollectionViewDataSource
    {
        protected List<ColorPickerShared.Model> CollectionItems;

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
            var cell = (ColorPickerCollectionViewCell)collectionView.DequeueReusableCell(ColorPickerCollectionViewCell.Identifier, indexPath) ?? ColorPickerCollectionViewCell.Create();
            cell.BindDataToCell(GetModelAtIndexPath(indexPath), indexPath);             return cell;
        } 
        public ColorPickerShared.Model GetModelAtIndexPath(NSIndexPath indexPath)
        {
            return CollectionItems[indexPath.Row];
        }
        public void SetCollectionItems(UICollectionView collectionView, List<ColorPickerShared.Model> items)
        {
            CollectionItems = items;
            collectionView.ReloadData();
        }
    } } 