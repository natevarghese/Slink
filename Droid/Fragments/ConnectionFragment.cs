
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Slink.Droid.Classes.Connections;

namespace Slink.Droid.Fragments
{
   public class ConnectionFragment : BaseFragment
    {
        protected RelativeLayout BackgroundView;
        protected RelativeLayout relative_Connection;
        protected TextView text_Connction;
        List<Card> cards;
        public RecyclerView recyclerViewConnection;
        public LinearLayoutManager linearLayoutManager;
        DiscoverShared discoverShared = new DiscoverShared();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Connection, container, false);
            relative_Connection = (RelativeLayout)view.FindViewById(Resource.Id.relativeShare);
            text_Connction = (TextView)view.FindViewById(Resource.Id.textConnection);
            BackgroundView = (RelativeLayout)view.FindViewById(Resource.Id.BackgroundView);
            recyclerViewConnection = (RecyclerView)view.FindViewById(Resource.Id.recyclerView);
            linearLayoutManager = new LinearLayoutManager(Context);
            recyclerViewConnection.SetLayoutManager(linearLayoutManager);
          
            if (DiscoverFragment.cardsList != null)
            {

                ConnectionRecyclerViewAdapter connectionRecycler = new ConnectionRecyclerViewAdapter(DiscoverFragment.cardsList);
                recyclerViewConnection.SetAdapter(connectionRecycler);
            }
            return view;
        }

    }
}
