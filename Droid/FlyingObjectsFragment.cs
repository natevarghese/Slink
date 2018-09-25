using System;
using Android.Animation;
using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace Slink.Droid
{
    public class FlyingObjectsFragment : BaseFragment
    {
        public bool Animating = false;
        LinearLayout Parent;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.FlyingObjects, container, false);

         
            Parent = view.FindViewById<LinearLayout>(Resource.Id.parent);



            return view;
        }
        public override void OnResume()
        {
            base.OnResume();
            StartAnimationLoop(AnimationDirection.right, Strings.DesignTypes.design_type_flying_colors, 60, 225, 2500);



          
        }

      


        public static void customView(View v, int cornerRadius, Android.Graphics.Color backgroundColor)
        {
            GradientDrawable shape = new GradientDrawable();
            shape.SetShape(ShapeType.Rectangle);
            shape.SetCornerRadius(cornerRadius);
            shape.SetColor(backgroundColor);
           
            v.Background = shape;
        }


        public enum AnimationDirection
        {
            right = 0,
            left = 1
        }

        public enum AnimationEnding
        {
            explode = 0,
            collapse = 1
        }
        async public void StartAnimationLoop(AnimationDirection direction, string designType, int minSize, int maxSize, float duration)
        {
            if (!String.IsNullOrEmpty(designType) && designType.Equals(Strings.DesignTypes.design_type_none, StringComparison.InvariantCultureIgnoreCase)) { EndAnimation(AnimationEnding.collapse, 0); return; }
           



            DisplayMetrics displayMetrics = new DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            Animating = true;
            Random random = new Random();

            //Queue q = new Queue();
            while (Animating)
            {
                var size = random.Next(minSize, maxSize);
                var y = random.Next(displayMetrics.HeightPixels);

                var view = new View(Activity);
                view.LayoutParameters = new LinearLayout.LayoutParams(size, size);
                view.SetX(-size);
                view.SetY(y);
                customView(view, size / 2, GetColor(random, designType));
                Parent.AddView(view, view.LayoutParameters);

                var o = ObjectAnimator.OfFloat(view, "x", displayMetrics.WidthPixels);
                o.SetInterpolator(new LinearInterpolator());
                o.SetDuration((long)duration);//miliseconds
                o.Start();
                o.AnimationEnd += (sender, e) =>
                {
                    //Parent.RemoveView(view);
                };
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(130));

            }
        }
        public void EndAnimation(AnimationEnding ending, float duration)
        {
            Parent.RemoveAllViews();
           
            //View.RemoveAllViews();
            //foreach (UIView view in Subviews)
            //{
            //    Animate(duration, delegate
            //    {
            //        if (ending == AnimationEnding.explode)
            //        {
            //            view.Layer.Frame = new CGRect(view.Frame.Location, new CGSize(view.Frame.Size.Width * 4, view.Frame.Size.Height * 4));
            //        }
            //        else
            //        {
            //            view.Layer.Frame = new CGRect(view.Frame.Location, new CGSize(0, 0));
            //        }
            //    }, delegate
            //    {
            //        view.RemoveFromSuperview();
            //    });

            //}

            Animating = false;
        }

        Android.Graphics.Color GetColor(Random random, string designType)
        {

            //if (designType.Equals(Strings.DesignTypes.design_type_flying_colors, StringComparison.InvariantCultureIgnoreCase)) return ColorUtils.GetRandomColor(random);
            //if (designType.Equals(Strings.DesignTypes.design_type_flying_lights, StringComparison.InvariantCultureIgnoreCase)) return Android.Graphics.Color.White;
            //if (designType.Equals(Strings.DesignTypes.design_type_none, StringComparison.InvariantCultureIgnoreCase)) 
            //{

            //    return ColorUtils.RemoveAllColor(random);
            //}
            //return Android.Graphics.Color.Transparent;
            if (String.IsNullOrEmpty(designType)) return ColorUtils.GetRandomColor(random);

            if (designType.Equals(Strings.DesignTypes.design_type_flying_colors, StringComparison.InvariantCultureIgnoreCase)) return ColorUtils.GetRandomColor(random);
            if (designType.Equals(Strings.DesignTypes.design_type_flying_lights, StringComparison.InvariantCultureIgnoreCase)) return Android.Graphics.Color.White;
            if (designType.Equals(Strings.DesignTypes.design_type_none, StringComparison.InvariantCultureIgnoreCase)) 
            {
                return ColorUtils.RemoveAllColor(random);
                

            }
            return Android.Graphics.Color.Transparent;
        }

    }
}
