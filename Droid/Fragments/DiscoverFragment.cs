using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Com.Wajahatkarim3.Easyflipview;
using static Android.Views.View;
using Context = Android.Content.Context;
                                                     
namespace Slink.Droid.Fragments
{
    public class DiscoverFragment : BaseFragment
    {
        Card Card;
        public static List<Card> cardsList;
        protected RelativeLayout BackgroundView, parentLayout;
        public RelativeLayout relative_Discover;
        public EasyFlipView easyFlipView;
        public Button yes_text_clicked, no_text_clicked;
        LinearLayout yes_no_view;
        RecyclerView RecyclerViewoutlet;
        RelativeLayout socialRelativeLayout;
        //Add data of frontcard
        public EditText userDisplayName_inCard, titleName_incard,Company_name;
        WebImageView image_in_cardfront, image_in_cardback;
       

        DiscoverShared Shared = new DiscoverShared();
       //ConnectionFragment objconnection = new ConnectionFragment();
        static bool Searching, ShouldStopSearching;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Discover, container, false);
            //relativeLayoutParent
            easyFlipView = (EasyFlipView)view.FindViewById(Resource.Id.FlipView);
            easyFlipView.Visibility = ViewStates.Invisible;
            parentLayout = (RelativeLayout)view.FindViewById(Resource.Id.relativeLayoutParent);
            //BackgroundView = (RelativeLayout)view.FindViewById(Resource.Id.BackgroundView);
            relative_Discover = (RelativeLayout)view.FindViewById(Resource.Id.relativeDiscover);
            yes_text_clicked = view.FindViewById<Button>(Resource.Id.yesbtn);
            no_text_clicked = view.FindViewById<Button>(Resource.Id.notbn);
            image_in_cardback = view.FindViewById<WebImageView>(Resource.Id.LogoImageView);
            Company_name = view.FindViewById<EditText>(Resource.Id.CompanyNameTextView);
            image_in_cardfront = view.FindViewById<WebImageView>(Resource.Id.HeaderImageView);
            userDisplayName_inCard = view.FindViewById<EditText>(Resource.Id.UserDisplayNameTextView);
            titleName_incard = view.FindViewById<EditText>(Resource.Id.TitleTextField);
            socialRelativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.socialRelativeLayout);
           RecyclerViewoutlet = view.FindViewById<RecyclerView>(Resource.Id.RecyclerView);
           yes_no_view = (LinearLayout)view.FindViewById(Resource.Id.linear_layout_bottom);
            if(relative_Discover!=null)
            StartSearching();
            return view;
        }

        /// <summary>
        /// Starts the searching.
        /// </summary>
        public async void StartSearching()
        {
            var anim = AnimationUtils.LoadAnimation(Context, Resource.Animator.scaleanim);
            relative_Discover.StartAnimation(anim);
            if (Searching) return;
            Searching = true;
            ShouldStopSearching = false;
            while (!ShouldStopSearching)
            {
                await Shared.GetNearbyTransactions();
                StopSearchingIfCardsFound();
                //Console.WriteLine("===========================GOT");
                await Task.Delay(TimeSpan.FromSeconds(5));
           
                //Console.WriteLine("====================DONE");
            }
        }
        /// <summary>
        /// Stops the searching if cards found.
        /// </summary>
        void StopSearchingIfCardsFound()
        {
            if (Shared.ShouldStopSearching())
            {
                HideCardViews(false);
                StopSearching();
                NextCardForCardView();
            }
            else
            {
                HideCardViews(true);
                StartSearching();

            }
        }
        /// <summary>
        ///  //insertion of data in card
        /// </summary>
        public void NextCardForCardView()
        {
            var item = Shared.GetNextCard();
            if (item == null)
            {
                Console.WriteLine("Item is null");
                return;
            }
            //add data in card
             //add card values into variables   
            var image_InCard = item.Card.RemoteLogoURL;
            userDisplayName_inCard.Text= item.Card.UserDisplayName;
            userDisplayName_inCard.Enabled = false;
            userDisplayName_inCard.TextSize = 13;
            titleName_incard.Text = item.Card.Title;
            titleName_incard.Enabled = false;
            titleName_incard.TextSize = 13;
            Company_name.Text = item.Card.CompanyName;
            Company_name.Enabled = false;
            image_in_cardback.SetImageResource( Resource.Drawable.ic_buildings);
            image_in_cardfront.SetImageResource(Resource.Drawable.ic_noprofile);
            Company_name.Text = item.Card.CompanyName;
            Company_name.TextSize = 13;
            easyFlipView.Visibility = ViewStates.Visible;
            yes_no_view.Visibility = ViewStates.Visible;
            cardsList = new List<Card>();
            cardsList.Add(item.Card);
            easyFlipView.Animate().ScaleY(1.2f).SetDuration(900).Start();
            easyFlipView.Animate().ScaleX(1.4f).SetDuration(900).Start();
            SelectYourOption();
            Console.WriteLine("Item is NOT null");
        }
        /// <summary>
        /// Selects your option.
        /// </summary>
        public void SelectYourOption()
        {
            yes_text_clicked.Click += delegate
            {
                Yesclicked();

            };

            no_text_clicked.Click += delegate
            {
                Noclicked();
             };


        }
        /// <summary>
        /// Stops the searching.
        /// </summary>
        public void StopSearching()
        {
            Searching = false;
            ShouldStopSearching = true;
            //end animation of Discover circle
            relative_Discover.Visibility = ViewStates.Invisible;
            relative_Discover.ClearAnimation();
        }
        /// <summary>
        /// Hides the card views.
        /// </summary>
        /// <param name="hidden">If set to <c>true</c> hidden.</param>
        void HideCardViews(bool hidden)
        {
            float finalResult = (hidden) ? 0 : 1;
            float oppositeResult = (hidden) ? 1 : 0;

        }
        #region for card accept or reject
        public void Yesclicked()
        {
            Shared.AcceptCard();
            StopSearchingIfCardsFound();
            yes_no_view.Visibility = ViewStates.Gone;
            easyFlipView.Visibility = ViewStates.Gone;
            Toast.MakeText(Context, "card added successfully", ToastLength.Long).Show();

        }
        public void Noclicked()
        {
            Shared.RejectCard();
            StopSearchingIfCardsFound();
            easyFlipView.Visibility = ViewStates.Gone;
            yes_no_view.Visibility = ViewStates.Gone;
           
        }
        #endregion

        public override void OnDetach()
        {
            relative_Discover.ClearAnimation();
            StopSearching();
            base.OnDetach();
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        public override void OnResume()
        {
            base.OnResume();
        }


    }
}
 