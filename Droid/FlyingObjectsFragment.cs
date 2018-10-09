using System;
using Android.Animation;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Android.Content;
using static Slink.Droid.ActionBroadcastReceiver;
using System.Drawing;
using Android.App;
using Android.Graphics;
using Android.Renderscripts;
using System.Runtime.CompilerServices;

namespace Slink.Droid
{
    public class FlyingObjectsFragment : BaseFragment, IOnDataRecievedListener
    {
        public static IOnDataRecievedListener onDataRecievedListener;
        public bool Animating = false, isFlyingColor, isFlyingLight;
        LinearLayout Parent;
        string selectedDesignType;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            isFlyingColor = true;
            onDataRecievedListener = this;
            var view = inflater.Inflate(Resource.Layout.FlyingObjects, container, false);
            Parent = view.FindViewById<LinearLayout>(Resource.Id.parent);
            StartAnimationLoop(AnimationDirection.right, Strings.DesignTypes.design_type_flying_colors, 20, 225, 2000);
            BlurTheView(view);
            return view;
        }

        private void BlurTheView(View view)
        {
            RenderScript rs = RenderScript.Create(this.Activity);
            var bitmap = GetBitmapFromView(view);
            var blurprocesor = new RSBlurProcessor(rs);
            var blurBitmap = blurprocesor.blur(bitmap, 15, 1);
            view.Background = new BitmapDrawable(blurBitmap);
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        /// <summary>
        /// Recieves the notification of design changed.
        /// </summary>
        public void RecieveData()
        {
            EndAnimation(AnimationEnding.collapse, 1);
            System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(20));
            StartFlyingObjectsView();
        }
        /// <summary>
        /// Starts the flying objects view.
        /// </summary>
        void StartFlyingObjectsView()
        {
            isFlyingColor = false;
            isFlyingLight = false;
            var service = ServiceLocator.Instance.Resolve<IPersistantStorage>();
            selectedDesignType = service.GetDesignType();
            if (selectedDesignType.Contains("Lights"))
                isFlyingLight = true;
            else if (selectedDesignType.Contains("Color"))
                isFlyingColor = true;
            else
                Animating = false;
            StartAnimationLoop(AnimationDirection.right, selectedDesignType, 20, 225, 2000);
        }

        public void CustomView(View v, int cornerRadius, Android.Graphics.Color backgroundColor, string designType, Random random)
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
        public async void StartAnimationLoop(AnimationDirection direction, string designType, int minSize, int maxSize, float duration)
        {
            if (!String.IsNullOrEmpty(designType) && designType.Equals(Strings.DesignTypes.design_type_none, StringComparison.InvariantCultureIgnoreCase)) { EndAnimation(AnimationEnding.collapse, 0); return; }
            DisplayMetrics displayMetrics = new DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            var random = new Random();

            while (isFlyingColor)
            {
                var size = random.Next(minSize, maxSize);
                var y = random.Next(displayMetrics.HeightPixels);
                var animationView = new View(Activity);
                animationView.LayoutParameters = new LinearLayout.LayoutParams(size, size);
                animationView.SetX(-size);
                animationView.SetY(y);
                CustomView(animationView, size / 2, GetColor(random, designType), designType, random);
                Parent.AddView(animationView, animationView.LayoutParameters);
                var objectAnimator = ObjectAnimator.OfFloat(animationView, "x", displayMetrics.WidthPixels);
                objectAnimator.SetInterpolator(new LinearInterpolator());
                objectAnimator.SetDuration((long)duration);//miliseconds
                objectAnimator.Start();
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(140));

            }
            while (isFlyingLight)
            {
                var size = random.Next(minSize, maxSize);
                var y = random.Next(displayMetrics.HeightPixels);
                var animationView = new View(Activity);
                animationView.LayoutParameters = new LinearLayout.LayoutParams(size, size);
                animationView.SetX(-size);
                animationView.SetY(y);
                CustomView(animationView, size / 2, Android.Graphics.Color.White, Strings.DesignTypes.design_type_flying_lights, random);
                Parent.AddView(animationView, animationView.LayoutParameters);
                var objectAnimator = ObjectAnimator.OfFloat(animationView, "x", displayMetrics.WidthPixels);
                objectAnimator.SetInterpolator(new LinearInterpolator());
                objectAnimator.SetDuration((long)duration);//miliseconds
                objectAnimator.Start();
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(140));
            }

        }
        public void EndAnimation(AnimationEnding ending, float duration)
        {

            Parent.RemoveAllViews();
            isFlyingLight = false;
            isFlyingColor = false;
            Animating = false;
        }

        Android.Graphics.Color GetColor(Random random, string designType)
        {

            if (string.IsNullOrEmpty(designType))
            {
                return ColorUtils.GetRandomColor(random);
            }
            else if (designType.Equals(Strings.DesignTypes.design_type_flying_colors))
                return ColorUtils.GetRandomColor(random);

            else if (designType.Equals(Strings.DesignTypes.design_type_flying_lights))
            {

                designType = Strings.DesignTypes.design_type_flying_lights;
                ColorUtils.RemoveAllColor(random);
                return Android.Graphics.Color.White;
            }
            else if (designType.Equals(Strings.DesignTypes.design_type_none))
                return ColorUtils.RemoveAllColor(random);

            return Android.Graphics.Color.Transparent;
        }

        public Bitmap GetBitmapFromView(View view)
        {
            DisplayMetrics displayMetrics = new DisplayMetrics();
            this.Activity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            int height = displayMetrics.HeightPixels;
            int width = displayMetrics.WidthPixels;
            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas c = new Canvas(bitmap);
            view.Layout(view.Left, view.Top, view.Right, view.Bottom);
            view.Draw(c);
            return bitmap;
        }
    }
}
