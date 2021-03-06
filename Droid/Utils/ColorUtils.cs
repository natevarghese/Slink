﻿using System;
using Android.Graphics;

namespace Slink.Droid
{
    public static class ColorUtils
    {
        public static Color GetRandomColor(Random random)
        {
            return Color.Argb(255, random.Next(256), random.Next(256), random.Next(256));
        }

        public static Color RemoveAllColor(Random random)
        {
            return Color.Argb(0, random.Next(0), random.Next(0), random.Next(0));


        }
        public static Color FromHexString(string hexValue, Color defaultColor)
        {
            if (String.IsNullOrEmpty(hexValue)) return defaultColor;

            return Color.ParseColor(hexValue);
        }

        public static string HexStringFromColor(Color color)
        {
            return Java.Lang.String.Format("#%06X", (0xFFFFFF & color));
        }
    }
}
