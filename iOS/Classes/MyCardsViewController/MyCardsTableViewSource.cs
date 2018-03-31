using System;
using UIKit;
using Foundation;
using CoreAnimation;
using CoreGraphics;

namespace Slink.iOS
{
    public class MyCardsTableViewSource : BaseTableViewSource<Card>
    {
        int ScrollMinimumLimit = 5;
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(MyCardsTableViewCell.Key) as MyCardsTableViewCell;
            var item = TableItems[indexPath.Row];

            if (cell == null)
                cell = MyCardsTableViewCell.Create();

            cell.BindDataToView(item, true, indexPath);

            return cell;
        }
        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return CardViewController.GetCalculatedHeight();
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return false;
        }
        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            if (TableItems.Count < ScrollMinimumLimit) return;
            if (willDecelerate) return;

            AnimateToIdentity(scrollView as UITableView);
        }
        public override void DecelerationEnded(UIScrollView scrollView)
        {
            if (TableItems.Count < ScrollMinimumLimit) return;

            AnimateToIdentity(scrollView as UITableView);
        }
        public override void Scrolled(UIScrollView scrollView)
        {
            if (TableItems == null || TableItems.Count < ScrollMinimumLimit) return;

            int result = (int)Math.Round((Double)((int)Math.Round(scrollView.ContentOffset.Y * 100000) % (int)Math.Round(scrollView.Frame.Size.Height * 100000))) / 100000;
            if (result == 0) return;

            var visibleCells = (scrollView as UITableView).VisibleCells;
            foreach (UITableViewCell cell in visibleCells)
            {
                UIView.BeginAnimations("float");
                UIView.SetAnimationDuration(0.1);
                ApplyRotation(cell, result);
                UIView.CommitAnimations();
            }

        }
        void AnimateToIdentity(UITableView tableView)
        {
            var visibleCells = tableView.VisibleCells;
            foreach (UITableViewCell cell in visibleCells)
            {
                UIView.BeginAnimations("float");
                UIView.SetAnimationDuration(0.15);
                cell.Layer.Transform = CATransform3D.Identity;
                UIView.CommitAnimations();
            }
        }
        void ApplyRotation(UIView view, int factor)
        {
            CATransform3D rotationAndPerspectiveTransform = CATransform3D.Identity;
            rotationAndPerspectiveTransform.m34 = (nfloat)(1.0 / -5000);
            //view.Layer.AnchorPoint = new CGPoint(0.5, 0);
            double angle = Math.Sqrt((factor * factor)) * 45.0 / 50.0;
            rotationAndPerspectiveTransform = rotationAndPerspectiveTransform.Rotate((nfloat)(-(angle <= 45 ? angle : 45.0) * Math.PI / 180.0f), 1.0f, 0.0f, 0.0f);
            rotationAndPerspectiveTransform = rotationAndPerspectiveTransform.Scale(0.9f);
            view.Layer.Transform = rotationAndPerspectiveTransform;
        }
    }
}
