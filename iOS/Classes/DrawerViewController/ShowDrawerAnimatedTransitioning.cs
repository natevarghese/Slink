using System;
using UIKit;
using CoreGraphics;

namespace Slink.iOS
{
    public class ShowDrawerAnimatedTransitioning : UIViewControllerAnimatedTransitioning
    {
        public bool IsPresenting;
        DrawerDirection Direction;
        int MaxX;

        public ShowDrawerAnimatedTransitioning(DrawerDirection direction, int maxX)
        {
            Direction = direction;
            MaxX = maxX;
        }

        public override double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
        {
            return 0.25;
        }
        public override void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {

            if (IsPresenting)
            {
                UIViewController toVC = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);
                FlyingObjectsContainterViewController fromVC = (FlyingObjectsContainterViewController)transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
                OpenDrawer(transitionContext, toVC, fromVC);
            }
            else
            {
                FlyingObjectsContainterViewController toVC = (FlyingObjectsContainterViewController)transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);
                UIViewController fromVC = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
                CloseDrawer(transitionContext, fromVC, toVC);
            }
        }

        private void OpenDrawer(IUIViewControllerContextTransitioning transitionContext, UIViewController toVC, FlyingObjectsContainterViewController fromVC)
        {
            transitionContext.ContainerView.AddSubview(toVC.View);
            var container = (fromVC as FlyingObjectsContainterViewController).ContainerNavigationController;
            var height = container.View.Window.Frame.Height - (container.View.Window.Frame.Height - container.View.Frame.GetMaxY());

            if (Direction == DrawerDirection.Left)
            {
                toVC.View.Frame = new CGRect(-fromVC.View.Frame.Size.Width, UIApplication.SharedApplication.StatusBarFrame.Size.Height, MaxX, height);
                fromVC.View.LayoutIfNeeded();
                fromVC.ShiftContainerRight(270);

                UIView.Animate(TransitionDuration(transitionContext),
                () =>
                {
                    if (fromVC.View.Frame.Size.Width - 50 > MaxX)
                    {
                        MaxX = 270;
                    }
                    toVC.View.Frame = new CGRect(0, UIApplication.SharedApplication.StatusBarFrame.Size.Height, MaxX, height);
                    fromVC.View.LayoutIfNeeded();
                },
                () =>
                {
                    transitionContext.CompleteTransition(true);
                }
            );
            }
            else
            {
                toVC.View.Frame = new CGRect(fromVC.View.Frame.Size.Width, UIApplication.SharedApplication.StatusBarFrame.Size.Height, MaxX, height);
                fromVC.View.LayoutIfNeeded();
                fromVC.ShiftContainerRight(270);

                UIView.Animate(TransitionDuration(transitionContext),
                    () =>
                    {
                        toVC.View.Frame = new CGRect(fromVC.View.Frame.Size.Width - MaxX, UIApplication.SharedApplication.StatusBarFrame.Size.Height, MaxX, height);
                        fromVC.View.LayoutIfNeeded();
                    },
                    () =>
                    {
                        transitionContext.CompleteTransition(true);
                    }
                );
            }




        }
        private void CloseDrawer(IUIViewControllerContextTransitioning transitionContext, UIViewController fromVC, FlyingObjectsContainterViewController toVC)
        {
            if (Direction == DrawerDirection.Left)
            {
                toVC.View.LayoutIfNeeded();
                toVC.ShiftContainerRight(0);
                UIView.Animate(0.25,
                    () =>
                    {
                        fromVC.View.Frame = new CGRect(-toVC.View.Frame.Size.Width, UIApplication.SharedApplication.StatusBarFrame.Size.Height, fromVC.View.Frame.Size.Width, toVC.View.Window.Frame.Size.Height);
                        toVC.View.LayoutIfNeeded();
                    },
                    () =>
                    {
                        transitionContext.CompleteTransition(true);
                    }
            );
            }
            else
            {
                toVC.View.LayoutIfNeeded();
                toVC.ShiftContainerRight(0);
                UIView.Animate(0.25,
                    () =>
                    {
                        fromVC.View.Frame = new CGRect(toVC.View.Frame.Size.Width, UIApplication.SharedApplication.StatusBarFrame.Size.Height, fromVC.View.Frame.Size.Width, toVC.View.Window.Frame.Size.Height);
                        toVC.View.LayoutIfNeeded();
                    },
                    () =>
                    {
                        fromVC = null;
                        transitionContext.CompleteTransition(true);
                    });

            }
        }
    }
    public enum DrawerDirection
    {
        Left = 0,
        Right = 1,
    }
}