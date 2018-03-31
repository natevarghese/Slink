using Foundation;
using System;
using UIKit;
using ObjCRuntime;

namespace Slink.iOS
{
    public partial class LabelWithActivityIndicatorView : UIView
    {
        public LabelWithActivityIndicatorView(IntPtr handle) : base(handle) { }
        public static LabelWithActivityIndicatorView Create()
        {
            var arr = NSBundle.MainBundle.LoadNib("LabelWithActivityIndicatorView", null, null);
            var v = Runtime.GetNSObject<LabelWithActivityIndicatorView>(arr.ValueAt(0));

            return v;
        }

        public UIView BindDataToView(string textForLabel, bool hasSadEmoji, bool hasActivityIndicator = false, UIActivityIndicatorViewStyle activityIndicatorStyle = UIActivityIndicatorViewStyle.Gray)
        {
            this.Label.Text = (hasSadEmoji) ? textForLabel + " \ud83d\ude1f" : textForLabel;
            Label.SizeToFit();

            ActivityIndicatorView.ActivityIndicatorViewStyle = activityIndicatorStyle;

            if (!hasActivityIndicator)
            {
                ActivityIndicatorView.Hidden = true;
                LabelTrailingSpaceConstraint.Constant = 8;
            }

            return this;
        }
    }
}