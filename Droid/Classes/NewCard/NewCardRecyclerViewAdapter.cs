using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using static Android.Views.View;
using Com.Wajahatkarim3.Easyflipview;

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
                    return new CardCell(Context.LayoutInflater.Inflate(Resource.Layout.CardCell, null));
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
                    ((CardCell)holder).BindDataToView(Context, position, ((NewCardModel)item).SelectedCard, true, false);
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

            LeftEditText.TextChanged -= LeftEditText_TextChanged;
            LeftEditText.Click -= LeftEditText_Click;

            LeftEditText.Text = model.Title;
            LeftEditText.Hint = model.Placeholder;
            LeftEditText.Clickable = model.Editable;
            LeftEditText.Focusable = model.Editable;
            LeftEditText.Tag = position;

            if (!model.Editable)
                LeftEditText.Click += LeftEditText_Click;


            RightTextView.SetBackgroundColor(ColorUtils.FromHexString(model.ColorHexString, Color.Transparent));
            RightTextView.Visibility = String.IsNullOrEmpty(model.ColorHexString) ? ViewStates.Gone : ViewStates.Visible;
            RightTextViewSuperView.Visibility = String.IsNullOrEmpty(model.ColorHexString) ? ViewStates.Gone : ViewStates.Visible;

            ItemView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 200);

            if (!ItemView.HasOnClickListeners)
            {
                ItemView.Click += (sender, e) =>
                {
                    var intent = new Intent(SettingsShared.ItemClickedBroadcastReceiverKey);
                    intent.PutExtra(SettingsShared.ItemClickedBroadcastReceiverKeyPosition, position);
                    context.SendBroadcast(intent);
                };
            }

            LeftEditText.TextChanged += LeftEditText_TextChanged;
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

    public class ImageAndTextViewCell : RecyclerView.ViewHolder, View.IOnCreateContextMenuListener
    {
        public WebImageView LeftImageView { get; set; }
        public TextView RightTextView { get; set; }
        public int MyPosition { get; set; }

        public ImageAndTextViewCell(View v) : base(v)
        {
            LeftImageView = v.FindViewById<WebImageView>(Resource.Id.LeftImageView);
            RightTextView = v.FindViewById<TextView>(Resource.Id.RightTextView);
            v.SetOnCreateContextMenuListener(this);
        }

        public void BindDataToView(Context context, int position, NewCardModel model)
        {
            if (model == null) return;
            if (model.Outlet == null) return;

            MyPosition = position;
            LeftImageView.SetImage(model.Outlet.RemoteURL, -1, -1, null, WebImageView.DefaultCircleTransformation);
            RightTextView.Text = model.Outlet.Name;
            ItemView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 200);
            if (ItemView.HasOnClickListeners) return;
            ItemView.Click += (sender, e) =>
            {
                var intent = new Intent(SettingsShared.ItemClickedBroadcastReceiverKey);
                intent.PutExtra(SettingsShared.ItemClickedBroadcastReceiverKeyPosition, position);
                context.SendBroadcast(intent);
            };
            ItemView.SetOnCreateContextMenuListener(this);
        }


        public void BindDataToView(Context context, int position, Outlet model)
        {
            if (model == null) return;

            MyPosition = position;

            LeftImageView.SetImage(model.RemoteURL, -1, -1, null, WebImageView.DefaultCircleTransformation);

            RightTextView.Text = model.Name;

            ItemView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 200);
            if (!ItemView.HasOnClickListeners)
            {
                ItemView.Click += (sender, e) =>
                {
                    var intent = new Intent(SettingsShared.ItemClickedBroadcastReceiverKey);
                    intent.PutExtra(SettingsShared.ItemClickedBroadcastReceiverKeyPosition, position);
                    context.SendBroadcast(intent);
                };
            }
            ItemView.SetOnCreateContextMenuListener(null);
            if (!model.Type.Equals(Outlet.outlet_type_facebook, StringComparison.InvariantCultureIgnoreCase))
                ItemView.SetOnCreateContextMenuListener(this);
        }

        public void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            menu.Add(0, v.Id, MyPosition, "Delete");
        }
    }

    //public class ViewOnLongClickListener : Java.Lang.Object, View.IOnLongClickListener
    //{
    //    public int Position { get; set; }

    //    public ViewOnLongClickListener(int position)
    //    {
    //        Position = position;
    //    }
    //}



    public class CardCell : RecyclerView.ViewHolder
    {
        public static int GetCalculatedHeight(int width)
        {
            float ratio = 200f / 320f;
            float result = width * ratio;
            return (int)result;
        }

        public CardFront FrontView;
        public CardBack RearView;
        public EditText NameTextView;
        public TextView FlipTextView;
        EasyFlipView FlipView;
        Card Card;


        public CardCell(View v) : base(v)
        {
            NameTextView = v.FindViewById<EditText>(Resource.Id.NameTextView);
            FlipTextView = v.FindViewById<TextView>(Resource.Id.FlipTextView);
            FrontView = v.FindViewById<CardFront>(Resource.Id.FrontView);
            FlipView = v.FindViewById<EasyFlipView>(Resource.Id.FlipView);
            RearView = v.FindViewById<CardBack>(Resource.Id.RearView);
        }

        public void BindDataToView(Context context, int position, Card item, bool editable, bool useParentPosition)
        {
            Card = item;

            //set height
            var width = context.Resources.DisplayMetrics.WidthPixels - 20;
            var height = (int)GetCalculatedHeight(width);
            ItemView.LayoutParameters = new RelativeLayout.LayoutParams(width, height);
            if (item == null) return;
            NameTextView.TextChanged -= NameTextView_TextChanged;
            NameTextView.Text = item.Name.Equals(Strings.Basic.new_card, StringComparison.InvariantCultureIgnoreCase) ? null : item.Name;
            NameTextView.Hint = Strings.Basic.new_card;
            NameTextView.TextChanged += NameTextView_TextChanged;
            NameTextView.Enabled = editable;
            FlipTextView.Click -= FlipTextView_Click;
            FlipTextView.Click += FlipTextView_Click;
            //wire up swipe gestures
            var touchListner = new OnSwipeTouchListener(context, ToggleViews, ToggleViews, position);
            ItemView.SetOnTouchListener(touchListner);
            var pos = useParentPosition ? position : 0;
            FrontView.BindDataToView(item, editable, pos);
            RearView.BindDataToView(item, editable, pos);
           // ApplyFlippedStateToView();

        }

        public void FlipTextView_Click(object sender, EventArgs e)
        {
            ToggleViews();
        }


        public WebImageView GetUserImageView()
        {
            return FrontView?.GetUserImageView();
        }
        public WebImageView GetCompanyLogoImageView()
        {
            return RearView?.GetCompanyLogoImageView();
        }
        public void FocusOnName()
        {
            if (NameTextView == null) return;
            if (NameTextView.Context == null) return;

            NameTextView.RequestFocus();

            var activity = NameTextView.Context as BaseActivity;
            activity?.ShowKeyboard(NameTextView);
        }
        void NameTextView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            Card.UpdateStringProperty(() => Card.Name, e.Text.ToString().Trim());
        }


        public void ToggleViews()
        {
          //  Card.Flip();
            FlipView.FlipTheView();
            FlipView.FlipDuration = 1000;

        }
        public void ApplyFlippedStateToView()
        {
            FrontView.Visibility = Card.IsFlipped ? ViewStates.Invisible : ViewStates.Visible;
            RearView.Visibility = Card.IsFlipped ? ViewStates.Visible : ViewStates.Invisible;
        }

        internal void FlipTextView_Click()
        {
            ToggleViews();
        }
    }


    public class OnSwipeTouchListener : Java.Lang.Object, IOnTouchListener
    {
        GestureDetector GestureDetector;

        public OnSwipeTouchListener(Context context, Action onSwipeLeft, Action onSwipeRight, int position)
        {
            var listener = new GestureListener(context);
            listener.OnSwipeLeft = onSwipeLeft;
            listener.OnSwipeRight = onSwipeRight;
            listener.Position = position;
            GestureDetector = new GestureDetector(context, listener);
        }
        public bool OnTouch(View v, MotionEvent e)
        {
            return GestureDetector.OnTouchEvent(e);
        }
        /// <summary>
        /// Swipe Gesture listener.
        /// </summary>
        class GestureListener : GestureDetector.SimpleOnGestureListener
        {
            Context Context;
            private CardCell cardCell;
            const int SWIPE_THRESHOLD = 100;
            const int SWIPE_VELOCITY_THRESHOLD = 100;
            public Action OnSwipeLeft, OnSwipeRight, OnSwipeTop, OnSwipeBottom;
            public int Position;

            public GestureListener(CardCell Class)
            {
                cardCell = Class;
            }

            public GestureListener(Context context)
            {
                Context = context;
            }

            public override bool OnDown(MotionEvent e)
            {
                return true;
            }
            public override bool OnSingleTapConfirmed(MotionEvent e)
            {
                var result = base.OnSingleTapConfirmed(e);
                var intent = new Intent(MyCardsShared.ItemClickedBroadcastReceiverKeyCardClicked);
                intent.PutExtra(MyCardsShared.ItemClickedBroadcastReceiverKeyPosition, Position);
                Context.SendBroadcast(intent);
                return result;
            }
            public override bool OnSingleTapUp(MotionEvent e)
            {
                return base.OnSingleTapUp(e);
            }

            public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                return true;
            }


            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                bool result = false;
                try
                {
                    float diffY = e2.GetY() - e1.GetY();
                    float diffX = e2.GetX() - e1.GetX();
                    if (Math.Abs(diffX) > Math.Abs(diffY))
                    {

                        if (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD)
                        {
                            if (diffX > 0)
                            {
                                OnSwipeRight?.Invoke();
                            }
                            else
                            {
                                OnSwipeLeft?.Invoke();
                            }
                            result = true;
                        }
                    }
                    else if (Math.Abs(diffY) > SWIPE_THRESHOLD && Math.Abs(velocityY) > SWIPE_VELOCITY_THRESHOLD)
                    {
                        if (diffY > 0)
                        {
                            OnSwipeBottom?.Invoke();
                        }
                        else
                        {
                            OnSwipeTop?.Invoke();
                        }

                        result = true;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }


                return result;
            }
        }
    }
}