using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    public class ColorPickerAppCompatActivity : BaseFragment
    {
        ColorPickerShared Shared = new ColorPickerShared();
        public Color StartingColor, SelectedColor;
        public Action<Color> ColorPicked;
        public string LabelTitle;

        View NewColorButton, GridViewParentView;
        GridView GridView;
        ColorPickerGridViewAdapter GridViewAdapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ColorPicker, container, false);

            var CurrentColorButton = view.FindViewById<View>(Resource.Id.CurrentColorButton);
            CurrentColorButton.SetBackgroundColor(StartingColor);

            NewColorButton = view.FindViewById<View>(Resource.Id.NewColorButton);


            Activity.Title = LabelTitle;
            CurrentColorButton.SetBackgroundColor(StartingColor);
            SelectedColor = StartingColor;


            GridView = view.FindViewById<GridView>(Resource.Id.GridView);
            GridView.Visibility = ViewStates.Invisible;
            GridView.ScrollBarStyle = ScrollbarStyles.OutsideOverlay;
            GridView.ItemClick += (sender, e) =>
            {
                var model = Shared.GetCollectionItems()[e.Position];

                var r = (model.color[0]);
                var g = (model.color[1]);
                var b = (model.color[2]);
                var a = (model.color[3]);

                var color = new Android.Graphics.Color(r, g, b, a);

                SelectedColor = color;
                UpdateCurrentColor();

                ((ColorPickerGridViewAdapter)GridView.Adapter).NotifyDataSetChanged();
            };


            GridViewParentView = view.FindViewById<View>(Resource.Id.GridViewParentView);



            GridViewAdapter = new ColorPickerGridViewAdapter();
            GridViewAdapter.Context = Activity;
            GridViewAdapter.ListItems = Shared.GetCollectionItems();
            GridView.Adapter = GridViewAdapter;

            //After ui is drawn Post is fired
            GridView.Post(() =>
            {
                LayoutGridView();
                GridView.Visibility = ViewStates.Visible;
            });

            UpdateCurrentColor();

            HasOptionsMenu = true;
            return view;
        }


        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            var listener = new OnGlobalLayoutListener();
            listener.OnGlobalLayoutAction += () =>
            {
                LayoutGridView();
                GridView.ViewTreeObserver.RemoveOnGlobalLayoutListener(listener);
            };
            GridView.ViewTreeObserver.AddOnGlobalLayoutListener(listener);

        }
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.toolbar_save, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.Save:
                    ColorPicked?.Invoke(SelectedColor);
                    Activity.SupportFragmentManager.PopBackStackImmediate();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                NewColorButton?.Dispose();
                NewColorButton = null;

                ColorPicked = null;
            }
        }
        int LayoutGridView()
        {
            //absoluteHeight. This is the height of the parent view. Should be used to calculate columnWidth and numColumns. The accumulated row heights cannot exceed this
            //584 is the minimum height allowed for 6 rows. Below this height should be 5 or less rows
            var absoluteHeight = GridViewParentView.Height;

            //columnWidth. The column width before padding in the gridview. 
            //Going below 80 would make the cells too small to be clickable. a cell is always square. unit is pixels
            var columnWidthPixals = 80;
            var verticalSpacing = GridView.VerticalSpacing;
            var minimumRowCount = 6;

            //figure out how many rows can fit reguardless of minimum
            var numOfRowsPotential = (int)Math.Floor((double)absoluteHeight / (columnWidthPixals + verticalSpacing));

            //we can fit more than minimumRowCount on the screen so make each cell larger
            if (numOfRowsPotential > minimumRowCount)
            {
                var numOfRowsPotentialNew = numOfRowsPotential;
                while (numOfRowsPotentialNew > minimumRowCount - 1)
                {
                    numOfRowsPotentialNew = (int)Math.Floor((double)absoluteHeight / (columnWidthPixals + verticalSpacing));
                    columnWidthPixals++;
                }
            }

            //cell do not exceed minimumRowCount rows
            var numOfRowsCalculated = (int)Math.Min(numOfRowsPotential, minimumRowCount);

            //numColumns. variable based on the amount of row space we can have.  
            //knowing the amount of rows, we can calculate the number of columns
            var numColumns = Shared.GetCollectionItems().Count / numOfRowsCalculated;

            //absoluteWidth. This is the width of the grid view. The number of columns * some extra to account for spacing should equal this. 
            //When the number of columns increases this should also increase, keeping the minimim column width in mind.
            var absoluteWidth = numColumns * (columnWidthPixals + ViewUtils.DpToPx(Activity, verticalSpacing));


            GridView.StretchMode = StretchMode.StretchSpacing;
            GridView.NumColumns = numColumns;
            GridView.SetColumnWidth(columnWidthPixals);

            var pas = GridView.LayoutParameters;
            pas.Width = (int)absoluteWidth;
            pas.Height = GridViewParentView.Height;
            GridView.LayoutParameters = pas;

            GridViewAdapter.NotifyDataSetChanged();

            return numOfRowsCalculated;
        }
        void UpdateCurrentColor()
        {
            NewColorButton.SetBackgroundColor(SelectedColor);
        }
    }


    class ColorPickerGridViewAdapter : BaseAdapter
    {
        public Activity Context;
        public List<ColorPickerShared.Model> ListItems;

        public override int Count
        {
            get { return (ListItems == null) ? 0 : ListItems.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }
        public ColorPickerShared.Model GetItemInList(int position)
        {
            return ListItems[position];
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? Context.LayoutInflater.Inflate(Resource.Layout.ColorPickerCell, null);

            var model = ListItems[position];
            var r = (model.color[0]);
            var g = (model.color[1]);
            var b = (model.color[2]);
            var a = (model.color[3]);
            var color = new Android.Graphics.Color(r, g, b, a);

            ImageView imgView = view.FindViewById<ImageView>(Resource.Id.ImageView);
            var drawable = Context.Resources.GetDrawable(Resource.Drawable.RoundedCorners);
            drawable.SetColorFilter(color, PorterDuff.Mode.SrcIn);
            imgView.Background = drawable;

            var background = view.FindViewById<ImageView>(Resource.Id.BackgroundView);
            var drawable2 = Context.Resources.GetDrawable(Resource.Drawable.RoundedCorners);
            drawable2.SetColorFilter(Android.Graphics.Color.White, PorterDuff.Mode.SrcIn);
            background.Background = drawable2;

            return view;
        }
    }

    [Register("com.nvcomputers.slink.SquareRelativeLayout")]
    public class SquareRelativeLayout : RelativeLayout
    {
        public SquareRelativeLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        public SquareRelativeLayout(Context context) : base(context) { }
        public SquareRelativeLayout(Context context, IAttributeSet attrs) : base(context, attrs) { }
        public SquareRelativeLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }
        public SquareRelativeLayout(Context context, IAttributeSet attrs, int defStyle, int defStyleRes) : base(context, attrs, defStyle, defStyleRes) { }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, widthMeasureSpec);
        }
    }

    class OnGlobalLayoutListener : Java.Lang.Object, Android.Views.ViewTreeObserver.IOnGlobalLayoutListener
    {
        public Action OnGlobalLayoutAction;

        public void OnGlobalLayout()
        {
            OnGlobalLayoutAction?.Invoke();
            OnGlobalLayoutAction = null;
        }
    }
}