
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace Slink.Droid.Fragments
{
    public class DiscoverFragment : BaseFragment
    {
        protected RelativeLayout BackgroundView;
        protected RelativeLayout relative_Discover;

       

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)

        {

            var view = inflater.Inflate(Resource.Layout.Discover, container, false);

            BackgroundView = (RelativeLayout)view.FindViewById(Resource.Id.BackgroundView);
            relative_Discover = (RelativeLayout)view.FindViewById(Resource.Id.relativeDiscover);
            
           

            // Animation Of vibrating ring of whole circle//
            var anim = AnimationUtils.LoadAnimation(Context,Resource.Animator.scaleanim);
            relative_Discover.StartAnimation(anim);
            return view;
        }
    }
}
