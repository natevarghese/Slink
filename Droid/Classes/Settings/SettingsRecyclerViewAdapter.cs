using System;
using Android.App;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    public class SettingsRecyclerViewAdapter : BaseRecyclerViewAdapter<SettingsShared.SettingsModel>
    {
        public SettingsRecyclerViewAdapter(Activity context) : base(context) { }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new SettingsCell(Context.LayoutInflater.Inflate(Resource.Layout.TitleAndTextCell, null));
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

            var item = GetItemInList(position);
            if (item == null) return;

            switch (GetItemViewType(position))
            {
                case 0:
                    ((SettingsCell)holder).BindDataToView(Context, position, item);
                    break;
            }
        }
    }

    public class SettingsCell : RecyclerView.ViewHolder
    {
        public TextView LeftTextView { get; set; }
        public TextView RightTextView { get; set; }

        public SettingsCell(View v) : base(v)
        {
            LeftTextView = v.FindViewById<TextView>(Resource.Id.LeftTextView);
            RightTextView = v.FindViewById<TextView>(Resource.Id.RightTextView);
        }

        public void BindDataToView(Context context, int position, SettingsShared.SettingsModel item)
        {
            LeftTextView.Text = item.Title;
            RightTextView.Text = item.Value;

            ItemView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 150);

            if (ItemView.HasOnClickListeners) return;
            ItemView.Click += (sender, e) =>
            {
                var intent = new Intent(SettingsShared.ItemClickedBroadcastReceiverKey);
                intent.PutExtra(SettingsShared.ItemClickedBroadcastReceiverKeyPosition, position);
                context.SendBroadcast(intent);
            };
        }
    }
}