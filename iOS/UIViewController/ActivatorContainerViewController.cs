using System;
namespace Slink.iOS
{
    public class ActivatorContainerViewController : ContainerViewController
    {
        public ActivatorContainerViewController() : base()
        {
            TargetViewController = new ActivatorViewController();
        }
    }
}
