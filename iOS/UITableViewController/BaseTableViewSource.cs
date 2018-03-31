using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public class BaseTableViewSource<T> : UITableViewSource
    {
        public IList<T> TableItems;

        public Action<NSIndexPath, T> ItemSelected;
        public Action<NSIndexPath, T> ItemDeselected;
        public Action<NSIndexPath, T> RowDeleted;

        public BaseHeaderFooterViewController HeaderViewController, FooterViewController;

        public BaseTableViewSource() { }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            throw new Exception("You must subclass and override this function");
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (cell.RespondsToSelector(new ObjCRuntime.Selector("setSeparatorInset:")))
            {
                cell.SeparatorInset = UIEdgeInsets.Zero;
            }
            if (cell.RespondsToSelector(new ObjCRuntime.Selector("setPreservesSuperviewLayoutMargins:")))
            {
                cell.PreservesSuperviewLayoutMargins = false;
            }
            if (cell.RespondsToSelector(new ObjCRuntime.Selector("setLayoutMargins:")))
            {
                cell.LayoutMargins = UIEdgeInsets.Zero;
            }
        }
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return (TableItems == null) ? 0 : TableItems.Count;
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            ItemSelected?.Invoke(indexPath, TableItems[indexPath.Row]);
        }
        public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            ItemDeselected?.Invoke(indexPath, TableItems[indexPath.Row]);
        }
        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return (section == 0 && HeaderViewController != null) ? HeaderViewController.GetHeight() : 0.01f;
        }
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            return (section == 0 && HeaderViewController != null) ? HeaderViewController.View : null;
        }
        public override nfloat GetHeightForFooter(UITableView tableView, nint section)
        {
            return (section == 0 && FooterViewController != null) ? FooterViewController.GetHeight() : 0.01f;
        }
        public override UIView GetViewForFooter(UITableView tableView, nint section)
        {
            return (section == 0 && FooterViewController != null) ? FooterViewController.View : null;
        }
        public override bool CanEditRow(UIKit.UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public virtual T GetModelInList(NSIndexPath indexPath)
        {
            return TableItems[indexPath.Row];
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UIKit.UITableView tableView, NSIndexPath indexPath)
        {
            return UIKit.UITableViewCellEditingStyle.Delete;
        }
        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            if (editingStyle == UITableViewCellEditingStyle.Delete)
            {
                RowDeleted?.Invoke(indexPath, TableItems[indexPath.Row]);
                tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
            }
        }
        public void SetItems(UITableView tableView, IList<T> items)
        {
            TableItems = items;
            tableView.ReloadData();
        }

        public virtual bool IsEmpty()
        {
            return TableItems == null || TableItems.Count == 0;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (HeaderViewController != null)
                {
                    HeaderViewController.Dispose();
                    HeaderViewController = null;
                }
                if (FooterViewController != null)
                {
                    FooterViewController.Dispose();
                    FooterViewController = null;
                }

                ItemSelected = null;
                ItemDeselected = null;
                TableItems = null;
            }
        }
    }
}
