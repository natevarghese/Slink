using Foundation;
using System;
using UIKit;
using CoreAnimation;
using CoreGraphics;


namespace Slink.iOS
{
    public partial class FlyingObjectsView : UIView
    {
        public bool Animating = false;

        public FlyingObjectsView(IntPtr handle) : base(handle) { }
        public FlyingObjectsView(CGRect rect) : base(rect) { }

        public enum AnimationDirection
        {
            right = 0,
            left = 1
        }

        public enum AnimationEnding
        {
            explode = 0,
            collapse = 1
        }

        async public void StartAnimationLoop(AnimationDirection direction, string designType, int minSize, int maxSize, nfloat duration)
        {
            if (!String.IsNullOrEmpty(designType) && designType.Equals(Strings.DesignTypes.design_type_none, StringComparison.InvariantCultureIgnoreCase)) { EndAnimation(AnimationEnding.collapse, 0); return; }

            Animating = true;
            Random random = new Random();
            while (Animating)
            {
                nfloat size = random.Next(minSize, maxSize);
                UIView view = new UIView(new CGRect(-size, -size / 2 + random.Next((int)Frame.Size.Height), size, size));

                view.BackgroundColor = GetColor(random, designType);
                view.Layer.CornerRadius = view.Frame.Size.Width / 2;
                view.Layer.MasksToBounds = true;

                CABasicAnimation anim = CABasicAnimation.FromKeyPath("position.x");
                if (direction == AnimationDirection.right)
                {
                    anim.From = NSObject.FromObject(Frame.Size.Width + view.Frame.Size.Width);
                    anim.To = NSObject.FromObject(-Frame.Size.Width);
                }
                else if (direction == AnimationDirection.left)
                {
                    anim.From = NSObject.FromObject(-view.Frame.Size.Width);
                    anim.To = NSObject.FromObject(Frame.Size.Width + view.Frame.Size.Width);
                }


                anim.Duration = duration;
                anim.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseIn);
                view.Layer.AddAnimation(anim, "anim");

                InsertSubview(view, 0);

                await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(100));

            }
        }

        public void EndAnimation(AnimationEnding ending, nfloat duration)
        {
            //Slink why isnt corner radius being retained?
            foreach (UIView view in Subviews)
            {
                Animate(duration, delegate
                {
                    if (ending == AnimationEnding.explode)
                    {
                        view.Layer.Frame = new CGRect(view.Frame.Location, new CGSize(view.Frame.Size.Width * 4, view.Frame.Size.Height * 4));
                    }
                    else
                    {
                        view.Layer.Frame = new CGRect(view.Frame.Location, new CGSize(0, 0));
                    }
                }, delegate
                {
                    view.RemoveFromSuperview();
                });

            }

            Animating = false;
        }

        UIColor GetColor(Random random, string designType)
        {
            if (String.IsNullOrEmpty(designType)) return ColorUtils.GetRandomColor(random);

            if (designType.Equals(Strings.DesignTypes.design_type_flying_colors, StringComparison.InvariantCultureIgnoreCase)) return ColorUtils.GetRandomColor(random);
            if (designType.Equals(Strings.DesignTypes.design_type_flying_lights, StringComparison.InvariantCultureIgnoreCase)) return UIColor.White;
            if (designType.Equals(Strings.DesignTypes.design_type_none, StringComparison.InvariantCultureIgnoreCase)) throw new Exception();

            return UIColor.Clear;
        }
    }
}