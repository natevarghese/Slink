using System;
using UIKit;
using Plugin.DeviceInfo;

namespace Slink.iOS
{
	public static class DeviceUtils
	{
		public static bool IsSimulator()
		{
			return UIDevice.CurrentDevice.Name.Equals("Nate’s MacBook Pro", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
