using System;

using Foundation;
using UIKit;

namespace Slink.iOS
{
    public partial class ColorPickerCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Identifier = new NSString("ColorPickerCollectionViewCell");
        public static readonly UINib Nib;

        public ColorPickerCollectionViewCell(IntPtr handle) : base(handle) { }

        public static ColorPickerCollectionViewCell Create()
        {
            return (ColorPickerCollectionViewCell)Nib.Instantiate(null, null)[0];
        }

        public void BindDataToCell(ColorPickerShared.Model model, NSIndexPath indexPath)
        {
            var r = (nfloat)(model.color[0] / 255f);
            var g = (nfloat)(model.color[1] / 255f);
            var b = (nfloat)(model.color[2] / 255f);
            var a = (nfloat)(model.color[3] / 255f);
            var color = UIColor.FromRGBA(r, g, b, a);
            ContentView.BackgroundColor = color;

            Layer.BorderColor = UIColor.White.CGColor;
            Layer.BorderWidth = 2;
            Layer.CornerRadius = 5;
        }
    }
}
