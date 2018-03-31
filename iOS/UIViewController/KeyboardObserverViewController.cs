using System;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public class KeyboardObserverViewController : NetworkListenerViewController
    {
        public KeyboardObserverViewController() : base() { }
        public KeyboardObserverViewController(IntPtr handle) : base(handle) { }
        public KeyboardObserverViewController(string xibName) : base(xibName) { }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}


