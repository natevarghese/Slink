using System;
using UIKit;
using CoreGraphics;
using Foundation;

namespace Slink.iOS
{
    public abstract class BaseHeaderFooterViewController : BaseViewController
    {
        public BaseHeaderFooterViewController(IntPtr handle) : base(handle) { }
        public BaseHeaderFooterViewController(string xibName, NSBundle bundle) : base(xibName) { }

        protected bool Appeared = false;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            View.LayoutIfNeeded();
        }

        public abstract void Reset();
        public abstract bool ShowsWhenEmpty();
        public abstract nfloat GetHeight();
    }
}

