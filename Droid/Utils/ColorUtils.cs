using System;
using Android.Graphics;

namespace Slink.Droid
{
    public static class ColorUtils
    {
        public static Color GetRandomColor(Random random)
        {
            return Color.Argb(255, random.Next(256), random.Next(256), random.Next(256));
        }

        public static Color FromHexString(string hexValue, Color defaultColor)
        {
            if (String.IsNullOrEmpty(hexValue)) return defaultColor;

            return Color.ParseColor(hexValue);
        }
    }
}
