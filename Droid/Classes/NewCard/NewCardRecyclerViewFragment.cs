using System;
using Android.Content;

namespace Slink.Droid
{
    public class NewCardRecyclerViewFragment : RecyclerViewFragment<NewCardModel>
    {
        public NewCardShared Shared = new NewCardShared();

        ActionBroadcastReceiver RowEditedBroadcastReceiver;

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            RecyclerViewAdapter.SetListItems(Shared.GetTableItems());

            Activity.Title = " ";

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            RowEditedBroadcastReceiver = new ActionBroadcastReceiver();
            RowEditedBroadcastReceiver.NotificationReceived += (obj) =>
            {
                var position = obj.GetIntExtra(SettingsShared.ItemClickedBroadcastReceiverKeyPosition, -1);
                if (position == -1) return;

                var text = obj.GetStringExtra(SettingsShared.ItemClickedBroadcastReceiverKeyValue);
                if (String.IsNullOrEmpty(text)) return;

                var model = RecyclerViewAdapter.GetItemInList(position);
                if (model.Placeholder.Equals(NewCardShared.new_card_model_title_placeholder, StringComparison.InvariantCultureIgnoreCase))
                {
                    model.SelectedCard.UpdateStringProperty(() => model.SelectedCard.Title, text.Trim());
                    model.SelectedCard.ShowFront();
                }
                else if (model.Placeholder.Equals(NewCardShared.new_card_model_display_name_placeholder, StringComparison.InvariantCultureIgnoreCase))
                {
                    model.SelectedCard.UpdateStringProperty(() => model.SelectedCard.UserDisplayName, text.Trim());
                    model.SelectedCard.ShowFront();
                }

                else if (model.Placeholder.Equals(NewCardShared.new_card_model_company_name, StringComparison.InvariantCultureIgnoreCase))
                {
                    model.SelectedCard.UpdateStringProperty(() => model.SelectedCard.CompanyName, text.Trim());
                    model.SelectedCard.ShowBack();
                }

                Activity.RunOnUiThread(() =>
                {
                    RecyclerViewAdapter.SetListItems(Shared.GetTableItems());
                });
            };
            Activity.RegisterReceiver(RowEditedBroadcastReceiver, new IntentFilter(Strings.InternalNotifications.notification_table_row_editing_changed));

        }
        public override void OnPause()
        {
            base.OnPause();

            if (RowEditedBroadcastReceiver != null)
                Activity.UnregisterReceiver(RowEditedBroadcastReceiver);
        }
        public override void RecyclerView_ItemClick(NewCardModel obj, int position)
        {
            base.RecyclerView_ItemClick(obj, position);

            //Footer
            if (obj == null)
            {
                var convertedActivity = Activity as BaseActivity;
                if (convertedActivity == null) return;

                ((MainActivity)Activity).AddFragmentOver(new MyOutletsRecyclerViewFragment());
                return;
            }

            if (String.IsNullOrEmpty(obj.Title)) return;

            var frag = new ColorPickerAppCompatActivity();
            frag.ColorPicked += (color) =>
            {
                if (obj.Title.Equals(NewCardShared.new_card_model_border_color, StringComparison.InvariantCulture))
                {

                    return;
                }

                if (obj.Title.Equals(NewCardShared.new_card_model_background_color, StringComparison.InvariantCulture))
                {

                    return;
                }

                if (obj.Title.Equals(NewCardShared.new_card_model_company_name_text_color, StringComparison.InvariantCulture))
                {

                    return;
                }
            };
            frag.LabelTitle = obj.Title;
            frag.StartingColor = ColorUtils.FromHexString(obj.ColorHexString, Android.Graphics.Color.White);

            var convertedActiviy = (Activity as BaseActivity);
            convertedActiviy.AddFragmentOver(frag);
        }
        public override BaseRecyclerViewAdapter<NewCardModel> GetRecyclerViewAdapter()
        {
            var adapter = new NewCardRecyclerViewAdapter(Activity);
            return adapter;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                RowEditedBroadcastReceiver?.Dispose();
                RowEditedBroadcastReceiver = null;
            }
        }
    }
}
