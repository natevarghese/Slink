using System;

using UIKit;

namespace Slink.iOS
{
    public partial class AddRowViewController : BaseHeaderFooterViewController
    {
        public Action<bool> Clicked;
        public string LabelAddText;
        public string LabelDeleteText;

        public AddRowViewController() : base("AddRowViewController", null) { }

        public override void ViewDidLoad()
        {
            NetworkListenerEnabled = false;

            base.ViewDidLoad();

            Label.Text = LabelAddText;
        }
        partial void AddRowClicked(Foundation.NSObject sender)
        {
            Clicked?.Invoke(Editing);
        }
        public override void SetEditing(bool editing, bool animated)
        {
            base.SetEditing(editing, animated);

            Label.TextColor = (editing) ? UIColor.Red : UIColor.White;
            Label.Text = (editing) ? LabelDeleteText : LabelAddText;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Clicked = null;
            }
        }

        public override bool ShowsWhenEmpty()
        {
            return true;
        }

        public override nfloat GetHeight()
        {
            return 62;
        }

        public override void Reset()
        {
            SetEditing(false, false);
        }
    }
}

