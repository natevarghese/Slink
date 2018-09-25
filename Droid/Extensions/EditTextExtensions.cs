using System;
using Android.Content.Res;
using Android.Widget;

namespace Slink.Droid
{
    public static class EditTextExtensions
    {
        public static void SetInvalid(this EditText editText, Resources resources)
        {
            editText.Background = resources.GetDrawable(Resource.Drawable.RoundedCornersWithThinBorderRed);
        }
    }
}
