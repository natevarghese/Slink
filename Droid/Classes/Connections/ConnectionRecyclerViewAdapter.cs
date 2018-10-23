using System;
using System.Collections.Generic;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Wajahatkarim3.Easyflipview;
using static Android.Support.V7.Widget.RecyclerView;
namespace Slink.Droid.Classes.Connections
{
	public class ConnectionRecyclerViewAdapter : RecyclerView.Adapter
    {
        List<Card> cards = new List<Card>();
        DiscoverShared Shared = new DiscoverShared();
       
        public ConnectionRecyclerViewAdapter(System.Collections.Generic.List<Card> cards)
        {
            this.cards = cards;
        }

        public override int ItemCount => cards.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyViewHolder myViewHolder = holder as MyViewHolder;
            myViewHolder.titleName_incard.Text = cards[position].CompanyNameTextColor;
            myViewHolder.userDisplayName_inCard.Text = cards[position].UserDisplayName;
            myViewHolder.titleName_incard.Text = cards[position].Title;
            myViewHolder.Company_name.Text = cards[position].CompanyName;
            myViewHolder.Company_name.Enabled = false;
            myViewHolder.userDisplayName_inCard.Enabled = false;
            myViewHolder.titleName_incard.Enabled = false;
            myViewHolder.company_image.SetImageResource(Resource.Drawable.ic_buildings);
            myViewHolder.header_image.SetImageResource(Resource.Drawable.ic_noprofile);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.connectionCardCell, null);
            MyViewHolder myViewHolder = new MyViewHolder(view);

            return myViewHolder;
        }

    }
        public class MyViewHolder : RecyclerView.ViewHolder
        {
            public EasyFlipView easyFlipView;
            public TextView Company_name { get; private set; }
            public TextView userDisplayName_inCard { get; private set; }
            public TextView titleName_incard { get; private set; }
            public WebImageView header_image { get; private set; }
            public WebImageView company_image { get; private set; }
            
            public MyViewHolder(View view) : base(view)
            {
                easyFlipView = (EasyFlipView)view.FindViewById(Resource.Id.FlipViewinConnec);
                Company_name = (TextView)view.FindViewById(Resource.Id.CompanyNameTextView);
                userDisplayName_inCard = (TextView)view.FindViewById(Resource.Id.UserDisplayNameTextView);
                titleName_incard = (TextView)view.FindViewById(Resource.Id.TitleTextField);
                header_image = (WebImageView)view.FindViewById(Resource.Id.HeaderImageView);
                company_image = (WebImageView)view.FindViewById(Resource.Id.LogoImageView);

             }
           

        }
}
