using System;
namespace Slink
{
    public static class ColorUtils
    {
        public static int[] GetColor(ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.LandingTextFieldNormal:
                    return new int[] { 170, 170, 170 };
                case ColorType.LandingTextFieldError:
                    return new int[] { 255, 0, 0 };
                case ColorType.LandingButton:
                    return new int[] { 1, 219, 3 };
                case ColorType.LandingButtonDisabled:
                    return new int[] { 189, 189, 189 };
                case ColorType.DefaultText:
                    return new int[] { 255, 255, 255 };
                case ColorType.Theme:
                    return new int[] { 41, 203, 205 };

                case ColorType.Black:
                    return new int[] { 0, 0, 0 };
                case ColorType.White:
                    return new int[] { 255, 255, 255 };



                default:
                    break;
            }
            return new int[] { 255, 255, 255 };
        }



        public enum ColorType
        {
            LandingTextFieldError = 1,
            LandingTextFieldNormal = 2,
            LandingButton = 3,
            LandingButtonDisabled = 4,
            DefaultText = 5,
            Theme = 6,

            Black = 100,
            White = 101,

            fake = -1000
        }


        public static string ToHexString(int[] color)
        {
            var r = Int32.Parse(Math.Floor((decimal)(color[0])).ToString()).ToString("X2");
            var g = Int32.Parse(Math.Floor((decimal)(color[1])).ToString()).ToString("X2");
            var b = Int32.Parse(Math.Floor((decimal)(color[2])).ToString()).ToString("X2");
            return "#" + r + g + b;

        }
    }
}
