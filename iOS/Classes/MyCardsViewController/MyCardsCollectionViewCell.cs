using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Linq;

namespace Slink.iOS
{
    public partial class MyCardsCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("MyCardsCollectionViewCell");
        public static readonly UINib Nib;

        protected MyCardsCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public void UpdatePercentage(nfloat percentage)
        {
            var percentageCircleView = ContentView.Subviews.Where(c => c.GetType() == typeof(PercentageCircleView)).FirstOrDefault() as PercentageCircleView;
            if (percentageCircleView == null) return;

            percentageCircleView.Draw(ContentView.Frame, percentage);
        }


        public void Reset()
        {
            Image.TintColor = UIColor.Clear;
            Image.Image = null;
        }

        public void BindDataToView(Outlet model)
        {
            Reset();

            Image.SetImage(model.RemoteURL, null, null, null);
        }
    }
}
