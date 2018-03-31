using System;
using System.Linq;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using UIKit;

namespace Slink.iOS
{
    public partial class LandingTabbarController : UITabBarController
    {
        public LandingTabbarController(IntPtr handle) : base(handle) { }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            foreach (UIViewController vc in ViewControllers)
            {
                vc.View.BackgroundColor = UIColor.Clear;
                vc.View.Alpha = 0;
            }

            View.BackgroundColor = UIColor.Clear;
            TabBar.Hidden = true;

        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            UIView.Animate(1, delegate
            {
                ViewControllers[0].View.Alpha = 1;
            });

            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            CrossConnectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
        }
        public UIViewController SetSelectedViewControllerByType(Type type, bool isNestedInNavigationController, Action completedHandler)
        {
            int index = -1;
            for (int i = 0; i < ViewControllers.Length; i++)
            {
                if (isNestedInNavigationController)
                {
                    var nc = ViewControllers[i] as UINavigationController;

                    if (nc == null)
                        continue;

                    if (nc.ViewControllers[0].GetType() == type)
                    {
                        index = i;
                        break;
                    }
                }
                else if (ViewControllers[i].GetType() == type)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                return SetSelectedIndex(index, completedHandler);
            }
            return null;
        }

        UIViewController SetSelectedIndex(int index, Action completedHandler)
        {
            UIView.Animate(.5, delegate
            {
                foreach (UIViewController vc in ViewControllers)
                {
                    vc.View.Alpha = 0;
                }
            }, delegate
            {
                SelectedIndex = index;
                UIView.Animate(.5, delegate
                {
                    ViewControllers[index].View.Alpha = 1;
                });
                if (completedHandler != null)
                {
                    completedHandler();
                }
            });
            return ViewControllers[index];
        }
        void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            //if (e.IsConnected)
            //{
            //    NVCSyncManager.GetInstance().Start();
            //}
            //else
            //{
            //    NVCSyncManager.GetInstance().Stop();
            //}
        }
    }
}