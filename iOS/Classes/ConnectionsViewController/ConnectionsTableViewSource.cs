using System;
using UIKit;
using Foundation;
using CoreAnimation;
using CoreGraphics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Slink.iOS
{
    public class ConnectionsTableViewSource : BaseTableViewSource<SlinkUser>
    {
        public Action<NSIndexPath, Card> CardSelected;

        IOrderedEnumerable<SlinkUser> GetOrderedSections()
        {
            return TableItems.DistinctBy(c => c.FacebookID).Where(c => c.Cards.Where(d => d.Retained == true && d.Deleted == false).Count() > 0).OrderBy(c => c.FirstName);
        }
        IQueryable<Card> GetCardsAtSection(nint section)
        {
            return GetOrderedSections().ElementAt((int)section).Cards.Where(c => c.Retained == true && c.Deleted == false);
        }
        public Card GetCardAtIndexPath(NSIndexPath indexPath)
        {
            return GetCardsAtSection(indexPath.Section).ToList().ElementAt(indexPath.Row);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return (TableItems == null) ? 0 : GetOrderedSections().Count();
        }
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            var count = GetCardsAtSection(section).Count();
            return (TableItems == null || TableItems.Count == 0) ? 0 : count;
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var target = GetCardAtIndexPath(indexPath);
            CardSelected?.Invoke(indexPath, target);
        }
        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 50;
        }
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var name = String.Empty;
            var user = GetOrderedSections().ToList().ElementAt((int)section);
            var firstCard = user.Cards.FirstOrDefault();
            if (firstCard != null)
                name = firstCard.Name;

            var view = new UIView(new CGRect(0, 0, tableView.Frame.Width, 50));
            var label = new UILabel(new CGRect(10, 0, view.Frame.Width - 10, 50));
            label.Text = name;
            label.TextColor = UIColor.White;
            label.Font = UIFont.FromName("Avenir-Medium", 18);
            view.AddSubview(label);

            return view;
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(MyCardsTableViewCell.Key) as MyCardsTableViewCell ?? MyCardsTableViewCell.Create();

            var item = GetCardAtIndexPath(indexPath);
            if (item == null) return cell;

            cell.BindDataToView(item, false, indexPath);

            return cell;
        }
        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return MyCardsTableViewCell.MissingNameHeight;
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return false;
        }
        public override bool IsEmpty()
        {
            return TableItems == null || TableItems.Count == 0 || GetOrderedSections().Count() == 0;
        }
        public override void DecelerationEnded(UIScrollView scrollView)
        {
            if (TableItems.Count < 5) return;

            AnimateToIdentity(scrollView as UITableView);
        }
        public override void Scrolled(UIScrollView scrollView)
        {
            if (TableItems.Count < 5) return;

            int result = (int)Math.Round((Double)((int)Math.Round(scrollView.ContentOffset.Y * 100000) % (int)Math.Round(scrollView.Frame.Size.Height * 100000))) / 100000;
            if (result == 0)
            {
                AnimateToIdentity(scrollView as UITableView);
            }
            else
            {
                var visibleCells = (scrollView as UITableView).VisibleCells;
                foreach (UITableViewCell cell in visibleCells)
                {
                    UIView.BeginAnimations("float");
                    UIView.SetAnimationDuration(0.1);
                    ApplyRotation(cell, result);
                    UIView.CommitAnimations();
                }
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
