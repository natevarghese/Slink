using System;
using UIKit;
using CoreGraphics;
using CoreAnimation;
using System.Linq;

namespace Slink.iOS
{
    public class PercentageCircleView : UIView
    {
        public PercentageCircleView()
        {
            BackgroundColor = UIColor.Clear;
        }

        public void Draw(CGRect rect, nfloat percentage)
        {
            Frame = rect;

            var circlePath = UIBezierPath.FromArc(new CGPoint(rect.Width / 2, rect.Height / 2), rect.Width / 2, (nfloat)(-0.5 * Math.PI), (nfloat)(1.5 * Math.PI), true);
            var circleShape = new CAShapeLayer();
            circleShape.Path = circlePath.CGPath;
            circleShape.StrokeColor = UIColor.Blue.CGColor;
            circleShape.FillColor = UIColor.White.CGColor;
            circleShape.LineWidth = 3.5f;
            circleShape.StrokeStart = 0;
            circleShape.StrokeEnd = percentage;

            Layer.Sublayers?.ToList().ForEach(p => p.RemoveFromSuperLayer());
            Layer.AddSublayer(circleShape);
        }
    }
}
