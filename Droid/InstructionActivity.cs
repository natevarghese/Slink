
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace Slink.Droid
{

    //This is the instruction screen.
    [Activity(Label = "InstructionActivity", Theme = "@style/NoActionBarTheme")]
    public class InstructionActivity : BaseActivity
    {
        public View FrontView2;
        public TextView aboutslinkapp, TextViewShare, aboutslinkapp1, TaptoshareTxtView, slinkinstruction, UserNameEdittext, UserTitleEditText, CardName, OutLet, Txtviewtaptoshare;
        public ImageView myImage, CircleRedImg, InsideImageView, ArrowImgView, UserIconImage, Contect, Facebook, Instagram, Linkedin, ArrowImageIcon, SharedCardMobileImg, OutSideImageView, LargerMobileimg;
        public RelativeLayout MainRelativeView, Main_Relative;
        public Button button, skipbtn;
        public Animation rotateAboutCornerAnimation, MoveCircle, fadeintext, fadeouttext, Arrowanimation, MobileAnimation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //getting id for instruction screen layout

            //var transaction = SupportFragmentManager.BeginTransaction();
            //transaction.Add(Resource.Id.under_fragment, new FlyingObjectsFragment());
            //transaction.Add(Resource.Id.over_fragment, new MyCardsRecyclerViewFragment());
            //transaction.Commit();

            SetContentView(Resource.Layout.instructionscreen);
            aboutslinkapp = FindViewById<TextView>(Resource.Id.aboutslink);
            aboutslinkapp1 = FindViewById<TextView>(Resource.Id.aboutslinkinfo);
            slinkinstruction = FindViewById<TextView>(Resource.Id.begin);
            TaptoshareTxtView = FindViewById<TextView>(Resource.Id.taptosharetextview);
            TextViewShare = FindViewById<TextView>(Resource.Id.txtviewshare);
            button = FindViewById<Button>(Resource.Id.btnStart);
            skipbtn = FindViewById<Button>(Resource.Id.skip);
            Main_Relative = FindViewById<RelativeLayout>(Resource.Id.mainrelativelayout);
            myImage = FindViewById<ImageView>(Resource.Id.imageview);

            FrontView2 = FindViewById<View>(Resource.Id.FrontView);
            ArrowImageIcon = FindViewById<ImageView>(Resource.Id.arrowimg);
            SharedCardMobileImg = FindViewById<ImageView>(Resource.Id.sharecardimg);
            OutSideImageView = FindViewById<ImageView>(Resource.Id.sharecardimg);
            LargerMobileimg = FindViewById<ImageView>(Resource.Id.phoneimg);
            InsideImageView = FindViewById<ImageView>(Resource.Id.inside_imageview);
            CircleRedImg = FindViewById<ImageView>(Resource.Id.redcircleimg);
            ArrowImgView = FindViewById<ImageView>(Resource.Id.arrowImgView);

            //geting id from InstructionscreenNext
            UserNameEdittext = FrontView2.FindViewById<TextView>(Resource.Id.UserDisplayNameTextView);
            UserTitleEditText = FrontView2.FindViewById<TextView>(Resource.Id.TitleTextField);
            MainRelativeView = FrontView2.FindViewById<RelativeLayout>(Resource.Id.main_layout);
            UserIconImage = FrontView2.FindViewById<ImageView>(Resource.Id.HeaderImageView);
            Contect = FrontView2.FindViewById<ImageView>(Resource.Id.contect);
            Facebook = FrontView2.FindViewById<ImageView>(Resource.Id.facebook);
            Instagram = FrontView2.FindViewById<ImageView>(Resource.Id.instagram);
            Linkedin = FrontView2.FindViewById<ImageView>(Resource.Id.linkedin);
            CardName = FrontView2.FindViewById<TextView>(Resource.Id.newcard);
            OutLet = FrontView2.FindViewById<TextView>(Resource.Id.outlet);


            //Animation for rotating logoicon and  fade text
            rotateAboutCornerAnimation = AnimationUtils.LoadAnimation(this, Resource.Animator.hyperspace);
            fadeintext = AnimationUtils.LoadAnimation(this, Resource.Animator.fadeout);
            fadeouttext = AnimationUtils.LoadAnimation(this, Resource.Animator.fadein);
            Arrowanimation = AnimationUtils.LoadAnimation(this, Resource.Animator.arrowanimate);
            MobileAnimation = AnimationUtils.LoadAnimation(this, Resource.Animator.moveMobileImg);
            MoveCircle = AnimationUtils.LoadAnimation(this, Resource.Animator.moveRedCircle);

            skipbtn.Click += delegate
            {
                //go to main activity when click skip button
                StartActivity(typeof(MainActivity));

            };

            button.Click += async delegate
            {
                aboutslinkapp.StartAnimation(fadeouttext);
                aboutslinkapp1.StartAnimation(fadeouttext);
                await Task.Delay(1500);
                myImage.StartAnimation(rotateAboutCornerAnimation);
                button.Visibility = ViewStates.Gone;
                await Task.Delay(1500);

                aboutslinkapp.ClearAnimation();
                aboutslinkapp1.ClearAnimation();

                await Task.Delay(1500);
                slinkinstruction.StartAnimation(fadeouttext);
                await Task.Delay(1000);
                slinkinstruction.ClearAnimation();

                FrontView2.Visibility = ViewStates.Visible;
                await Task.Delay(1500);
                aboutslinkapp.Text = "Add Your name and Picture";
                aboutslinkapp.StartAnimation(fadeouttext);

                await Task.Delay(1000);
                UserNameEdittext.Text = "";

                UserNameEdittext.Text = "j";

                await Task.Delay(400);
                UserNameEdittext.Append("o");

                await Task.Delay(400);
                UserNameEdittext.Append("h");


                await Task.Delay(400);
                UserNameEdittext.Append("n");
                await Task.Delay(100);


                UserNameEdittext.Append(" ");
                await Task.Delay(400);

                UserNameEdittext.Append("D");
                await Task.Delay(400);

                UserNameEdittext.Append("o");
                await Task.Delay(400);
                UserNameEdittext.Append("e");

                await Task.Delay(400);
                UserIconImage.SetImageResource(Resource.Drawable.instructionimage);
                await Task.Delay(1000);
                aboutslinkapp.Text = "Change the Border Color";
                aboutslinkapp.StartAnimation(fadeouttext);
                await Task.Delay(1000);
                //aboutslinkapp.ClearAnimation();
                MainRelativeView.Background = Resources.GetDrawable(Resource.Drawable.customcorner);
                MainRelativeView.SetBackgroundResource(Resource.Drawable.customcorner);

                await Task.Delay(1500);
                aboutslinkapp.Text = "Add your phone no";
                OutLet.Visibility = ViewStates.Gone;
                aboutslinkapp.StartAnimation(fadeouttext);
                //aboutslinkapp.ClearAnimation();
                await Task.Delay(1500);

                SetOtherAccountAsync();

                //setting card name


            };

            async Task SetOtherAccountAsync()
            {
                Contect.SetImageResource(Resource.Drawable.contect);
                Contect.Visibility = ViewStates.Visible;
                await Task.Delay(500);
                aboutslinkapp.Text = "Even your favourite social media accounts";
                aboutslinkapp.StartAnimation(fadeouttext);
                await Task.Delay(1500);
                //adding social accounts
                Facebook.SetImageResource(Resource.Drawable.facebook);
                Facebook.Visibility = ViewStates.Visible;
                Linkedin.SetImageResource(Resource.Drawable.linkedin);
                Linkedin.Visibility = ViewStates.Visible;
                Instagram.SetImageResource(Resource.Drawable.instagram);
                Instagram.Visibility = ViewStates.Visible;

                await Task.Delay(1500);
                aboutslinkapp.Text = "Don't forget to give your card a name";
                aboutslinkapp.StartAnimation(fadeouttext);
                await Task.Delay(1500);
                aboutslinkapp.ClearAnimation();
                SetCardNameAsync();

            }




        }
        public async Task SetCardNameAsync()
        {

            await Task.Delay(1000);

            CardName.Text = "S";

            await Task.Delay(400);
            CardName.Append("o");

            await Task.Delay(400);
            CardName.Append("c");


            await Task.Delay(400);
            CardName.Append("i");
            await Task.Delay(400);

            CardName.Append("a");
            await Task.Delay(400);

            CardName.Append("l");
            await Task.Delay(1500);

            MainRelativeView.Visibility = ViewStates.Gone;
            CardName.Visibility = ViewStates.Gone;


            TaptoshareTxtView.Visibility = ViewStates.Visible;
            await Task.Delay(1000);

            TaptoshareTxtView.Visibility = ViewStates.Visible;
            await Task.Delay(1000);

            SharedCardMobileImg.Visibility = ViewStates.Visible;
            InsideImageView.Visibility = ViewStates.Visible;
            ArrowImageIcon.Visibility = ViewStates.Visible;
            TextViewShare.Visibility = ViewStates.Visible;
            //ArrowImageIcon.StartAnimation(Arrowanimation);
            TaptoshareClick();


        }
        public async Task TaptoshareClick()
        {
            OutSideImageView.Click += async (s, e) =>
            {
                ArrowImageIcon.Visibility = ViewStates.Invisible;
                TextViewShare.Visibility = ViewStates.Invisible;
                await Task.Delay(1000);
                TaptoshareTxtView.Visibility = ViewStates.Invisible;

                LargerMobileimg.Visibility = ViewStates.Visible;
                await Task.Delay(1000);

                //animation for setting flying image
                AnimateImageViewAsync();

                //InsideImageView.StartAnimation(MobileAnimation);
                await Task.Delay(2000);

                SharedCardMobileImg.Visibility = ViewStates.Invisible;
                //InsideImageView.ClearAnimation();
                await Task.Delay(1500);
                TaptoshareTxtView.Text = "Get notified when people \n share with you";
                await Task.Delay(1500);
                TaptoshareTxtView.StartAnimation(fadeouttext);
                await Task.Delay(1500);
                TaptoshareTxtView.ClearAnimation();
                InsideImageView.Visibility = ViewStates.Invisible;
                await Task.Delay(1500);
                CircleRedImg.Visibility = ViewStates.Visible;
                CircleRedImg.StartAnimation(fadeouttext);
                CircleRedImg.StartAnimation(MoveCircle);
                await Task.Delay(1500);

                EndInstruction();

            };

            //removing unnecessary view
            async Task EndInstruction()
            {
                await Task.Delay(1500);
                LargerMobileimg.Visibility = ViewStates.Invisible;
                CircleRedImg.Visibility = ViewStates.Invisible;
                await Task.Delay(1500);
                CircleRedImg.ClearAnimation();
                ArrowImgView.Visibility = ViewStates.Visible;

                skipbtn.Text = "Let's Go";

            }


        }

        private async Task AnimateImageViewAsync()
        {
            int[] location = new int[2];
            LargerMobileimg.GetLocationOnScreen(location);
            int x = location[0] + LargerMobileimg.Height / 2; ;

            int y = location[1] + LargerMobileimg.Width / 2; ;


            int[] location1 = new int[2];
            InsideImageView.GetLocationOnScreen(location1);
            int x1 = location1[0] + InsideImageView.Height / 2;
            int y1 = location1[1] + InsideImageView.Width / 2;


            InsideImageView.Animate().TranslationXBy((x - x1) + 25).TranslationYBy((y - y1)).RotationBy(360).ScaleXBy(2.5f).ScaleYBy(2.5f)
        .SetDuration(4000)
        .Start();
            await Task.Delay(1500);

        }

        public override void UpdateToolbar()
        {

        }
    }
}
