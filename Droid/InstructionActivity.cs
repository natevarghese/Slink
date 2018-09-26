
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace Slink.Droid
{

    //This is the instruction screen.
    [Activity(Label = "InstructionActivity")]
    public class InstructionActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.instructionscreen);

            TextView aboutslinkapp = FindViewById<TextView>(Resource.Id.aboutslink);
            TextView slinkinstruction = FindViewById<TextView>(Resource.Id.begin);
            Button button = FindViewById<Button>(Resource.Id.btnStart);
            Button skipbtn = FindViewById<Button>(Resource.Id.skip);
            ImageView myImage = FindViewById<ImageView>(Resource.Id.imageview);

            //Animation for rotating logoicon and  fade text

            var rotateAboutCornerAnimation = AnimationUtils.LoadAnimation(this, Resource.Animator.hyperspace);
            var fadeintext = AnimationUtils.LoadAnimation(this, Resource.Animator.fadeout);
            var fadeouttext = AnimationUtils.LoadAnimation(this, Resource.Animator.fadein);
            button.Click += delegate
            {

                myImage.StartAnimation(rotateAboutCornerAnimation);
                aboutslinkapp.StartAnimation(fadeintext);


            };

            skipbtn.Click += delegate
            {
                //go to main activity when click skip button
                StartActivity(typeof(MainActivity));

            };





        }
    }
}
