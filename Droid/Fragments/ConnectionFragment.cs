
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Slink.Droid.Fragments
{
    public class ConnectionFragment : BaseFragment
    {
        protected RelativeLayout BackgroundView;
        protected RelativeLayout relative_Connection;
        protected TextView text_Connction;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Connection, container, false);

            relative_Connection = (RelativeLayout)view.FindViewById(Resource.Id.relativeShare);
            text_Connction = (TextView)view.FindViewById(Resource.Id.textConnection);
            BackgroundView = (RelativeLayout)view.FindViewById(Resource.Id.BackgroundView);


            return view;
        }
    }
}
