using System;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public class SystemEventsViewController : BaseViewController
    {
        public SystemEventsViewController() : base() { }
        public SystemEventsViewController(IntPtr handle) : base(handle) { }
        public SystemEventsViewController(string xibName) : base(xibName) { }

        NSObject WillEnterForgroundNotification;

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            WillEnterForgroundNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, WillEnterForground);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            if (WillEnterForgroundNotification != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(WillEnterForgroundNotification);
            }
        }

        virtual public void WillEnterForground(NSNotification obj)
        {

        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (WillEnterForgroundNotification != null)
                {
                    WillEnterForgroundNotification.Dispose();
                }
            }
        }
    }
}
