using System;
using UIKit;

namespace Slink.iOS
{
    public class ShowDrawerTransitionDelegate : UIViewControllerTransitioningDelegate
    {

        DrawerDirection Direction;
        int MaxX;

        public ShowDrawerTransitionDelegate(DrawerDirection direction, int maxX)
        {
            Direction = direction;
            MaxX = maxX;
        }


        public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController(UIViewController presented, UIViewController presenting, UIViewController source)
        {
            ShowDrawerAnimatedTransitioning controller = new ShowDrawerAnimatedTransitioning(Direction, MaxX);
            controller.IsPresenting = true;

            return controller;
        }

        public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(UIViewController dismissed)
        {
            dismissed.View.Layer.CornerRadius = 0;
            dismissed.View.Layer.ShadowOffset = new CoreGraphics.CGSize(0, 0);

            ShowDrawerAnimatedTransitioning controller = new ShowDrawerAnimatedTransitioning(Direction, MaxX);
            controller.IsPresenting = false;

            return controller;
        }
    }
}