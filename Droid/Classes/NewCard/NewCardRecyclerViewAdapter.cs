using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Nostra13.Universalimageloader.Core;

namespace Slink.Droid
{
    public class NewCardRecyclerViewAdapter : BaseRecyclerViewAdapter<NewCardModel>
    {
        public NewCardRecyclerViewAdapter(Activity context) : base(context) { }

        public override int GetItemViewType(int position)
        {
            var target = GetItemInList(position);
            if (target == null) return 0;

            if (target.IsHeader) return 1;
            else if (target.Outlet != null) return 3;
            else if (target.IsTitle) return 4;
            else return 2;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case 0:
                case 4:
                    return new MyCardsFooter(Context.LayoutInflater.Inflate(Resource.Layout.TextViewCell, null));
                case 1:
                    return new CardCell(Context.LayoutInflater.Inflate(Resource.Layout.TitleAndTextCell, null));
                case 2:
                    return new TitleAndAccessoryCell(Context.LayoutInflater.Inflate(Resource.Layout.TitleAndAccessoryCell, null));
                case 3:
                    return new ImageAndTextViewCell(Context.LayoutInflater.Inflate(Resource.Layout.ImageAndTextViewCell, null));
                default:
                    return null;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

            var item = GetItemInList(position);

            switch (GetItemViewType(position))
            {
                case 0:
                    ((MyCardsFooter)holder).BindDataToView(Context, position, NewCardShared.AddNewOutlet);
                    break;
                case 1:
                    ((CardCell)holder).BindDataToView(Context, position, ((NewCardModel)item).SelectedCard);
                    break;
                case 2:
                    ((TitleAndAccessoryCell)holder).BindDataToView(Context, position, item);
                    break;
                case 3:
                    ((ImageAndTextViewCell)holder).BindDataToView(Context, position, item);
                    break;
                case 4:
                    ((MyCardsFooter)holder).BindDataToView(Context, position, item.Title);
                    break;
            }
        }

        public class TitleAndAccessoryCell : RecyclerView.ViewHolder
        {
            public TextView RightTextView { get; set; }
            public TextView RightTextViewSuperView { get; set; }
            public EditText LeftEditText { get; set; }

            public TitleAndAccessoryCell(View v) : base(v)
            {
                RightTextViewSuperView = v.FindViewById<TextView>(Resource.Id.RightTextViewSuperView);
                RightTextView = v.FindViewById<TextView>(Resource.Id.RightTextView);
                LeftEditText = v.FindViewById<EditText>(Resource.Id.LeftEditText);
            }

            public void BindDataToView(Context context, int position, NewCardModel model)
            {
                if (model == null) return;

                LeftEditText.Text = model.Title;
                LeftEditText.Hint = model.Placeholder;
                LeftEditText.Clickable = model.Editable;
                LeftEditText.Focusable = model.Editable;
                LeftEditText.Tag = position;

                LeftEditText.Click -= LeftEditText_Click;
                if (!model.Editable)
                    LeftEditText.Click += LeftEditText_Click;

                LeftEditText.TextChanged -= LeftEditText_TextChanged;
                LeftEditText.TextChanged += LeftEditText_TextChanged;

                RightTextView.SetBackgroundColor(ColorUtils.FromHexString(model.ColorHexString, Color.Transparent));
                RightTextView.Visibility = String.IsNullOrEmpty(model.ColorHexString) ? ViewStates.Gone : ViewStates.Visible;
                RightTextViewSuperView.Visibility = String.IsNullOrEmpty(model.ColorHexString) ? ViewStates.Gone : ViewStates.Visible;

                ItemView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 200);

                if (ItemView.HasOnClickListeners) return;
                ItemView.Click += (sender, e) =>
                {
                    var intent = new Intent(SettingsShared.ItemClickedBroadcastReceiverKey);
                    intent.PutExtra(SettingsShared.ItemClickedBroadcastReceiverKeyPosition, position);
                    context.SendBroadcast(intent);
                };
            }

            void LeftEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
            {
                var editText = sender as EditText;
                if (editText == null) return;

                var position = (int)editText.Tag;

                var intent = new Intent(Strings.InternalNotifications.notification_table_row_editing_changed);
                intent.PutExtra(SettingsShared.ItemClickedBroadcastReceiverKeyPosition, position);
                intent.PutExtra(SettingsShared.ItemClickedBroadcastReceiverKeyValue, editText.Text);
                editText.Context.SendBroadcast(intent);
            }

            void LeftEditText_Click(object sender, EventArgs e)
            {
                var editText = sender as EditText;
                if (editText == null) return;

                var position = (int)editText.Tag;

                var intent = new Intent(SettingsShared.ItemClickedBroadcastReceiverKey);
                intent.PutExtra(SettingsShared.ItemClickedBroadcastReceiverKeyPosition, position);
                editText.Context.SendBroadcast(intent);
            }
        }
        public class ImageAndTextViewCell : RecyclerView.ViewHolder
        {
            public ImageView LeftImageView { get; set; }
            public TextView RightTextView { get; set; }

            public ImageAndTextViewCell(View v) : base(v)
            {
                LeftImageView = v.FindViewById<ImageView>(Resource.Id.LeftImageView);
                RightTextView = v.FindViewById<TextView>(Resource.Id.RightTextView);
            }

            public void BindDataToView(Context context, int position, NewCardModel model)
            {
                if (model == null) return;
                if (model.Outlet == null) return;

                ImageLoader.Instance.LoadImage(model.Outlet.RemoteURL, new ImageLoadingListener());

                RightTextView.Text = model.Outlet.Handle;

                ItemView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 200);

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
}