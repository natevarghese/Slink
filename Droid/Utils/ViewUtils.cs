using System;
using Android.App;

namespace Slink.Droid
{
    public static class ViewUtils
    {
        public static int DpToPx(Activity activity, float valueInDp)
        {
            return (int)Math.Round(valueInDp * activity.Resources.DisplayMetrics.Density);
        }

        public static int PxToDp(Activity activity, int px)
        {
            return (int)Math.Round(px / activity.Resources.DisplayMetrics.Density);
        }
    }
}
