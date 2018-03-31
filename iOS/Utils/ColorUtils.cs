using System;
using UIKit;
using CoreGraphics;
using CoreImage;

namespace Slink.iOS
{
    public static class ColorUtils
    {
        public static UIColor GetColor(Slink.ColorUtils.ColorType colorType)
        {
            int[] colors = Slink.ColorUtils.GetColor(colorType);
            return UIColor.FromRGB(colors[0], colors[1], colors[2]);
        }

        public static string HexStringFromColor(UIColor color)
        {
            var r = Int32.Parse(Math.Floor((decimal)(new CIColor(color).Red * 255)).ToString()).ToString("X2");
            var g = Int32.Parse(Math.Floor((decimal)(new CIColor(color).Green * 255)).ToString()).ToString("X2");
            var b = Int32.Parse(Math.Floor((decimal)(new CIColor(color).Blue * 255)).ToString()).ToString("X2");
            return "#" + r + g + b;
        }
        public static UIColor FromHexString(string hexValue, UIColor defaultColor)
        {
            if (defaultColor == null)
                defaultColor = UIColor.Clear;

            if (String.IsNullOrEmpty(hexValue))
                return defaultColor;


            var colorString = hexValue.Replace("#", "");
            float red, green, blue;

            switch (colorString.Length)
            {
                case 3: // #RGB
                    {
                        red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
                        green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
                        blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
                        return UIColor.FromRGB(red, green, blue);
                    }
                case 6: // #RRGGBB
                    {
                        red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                        green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                        blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                        return UIColor.FromRGB(red, green, blue);
                    }
                case 8: // #AARRGGBB
                    {
                        var alpha = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                        red = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                        green = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                        blue = Convert.ToInt32(colorString.Substring(6, 2), 16) / 255f;
                        return UIColor.FromRGBA(red, green, blue, alpha);
                    }
                default:
                    return defaultColor;

            }
        }
        public static UIColor CreateGradient(double percent, double topX, double bottomX, UIColor init, UIColor goal)
        {
            double t = (percent - bottomX) / (topX - bottomX);

            t = Math.Max(0.0, Math.Min(t, 1.0));

            nfloat[] cgInit = init.CGColor.Components;
            nfloat[] cgGoal = goal.CGColor.Components;

            nfloat r = (nfloat)(cgInit[0] + t * (cgGoal[0] - cgInit[0]));
            nfloat g = (nfloat)(cgInit[1] + t * (cgGoal[1] - cgInit[1]));
            nfloat b = (nfloat)(cgInit[2] + t * (cgGoal[2] - cgInit[2]));

            return UIColor.FromRGBA(r, g, b, 1.0f);
        }
        public static UIColor GetRandomColor(Random random)
        {
            int hue = random.Next(255);
            UIColor color = UIColor.FromHSB((hue / 255.0f), 1.0f, 1.0f);
            return color;
        }
    }
}

