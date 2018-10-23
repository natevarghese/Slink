using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Provider;
using Android.Renderscripts;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using FFImageLoading;

namespace Slink.Droid
{
    public class NewCardRecyclerViewFragment : RecyclerViewFragment<NewCardModel>
    {
        public NewCardShared Shared = new NewCardShared();

        ActionBroadcastReceiver RowEditedBroadcastReceiver, CardEditingChangedBroadcaseReceiver, NoOutletsTappedBroadcastReceiver;
        ActionBroadcastReceiver CardUserImageClickedBroadcastReceiver, CardCompanyLogoImageClickedBroadcastReceiver;

        public override View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            RecyclerViewAdapter.SetListItems(Shared.GetTableItems());

            Activity.Title = "Slink";

            HasOptionsMenu = true;

            BlurTheView(view);
            return view;
        }

        public override void OnStart()
        {
            base.OnStart();

            RowEditedBroadcastReceiver = new ActionBroadcastReceiver();
            RowEditedBroadcastReceiver.NotificationReceived += (obj) =>
            {
                var position = obj.GetIntExtra(SettingsShared.ItemClickedBroadcastReceiverKeyPosition, -1);
                if (position == -1) return;

                var text = obj.GetStringExtra(SettingsShared.ItemClickedBroadcastReceiverKeyValue);
                if (String.IsNullOrEmpty(text)) return;

                var model = RecyclerViewAdapter.GetItemInList(position);
                if (String.IsNullOrEmpty(model.Placeholder)) return;

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
                    RecyclerViewAdapter.NotifyItemChanged(0);
                });
            };
            Activity.RegisterReceiver(RowEditedBroadcastReceiver, new IntentFilter(Strings.InternalNotifications.notification_table_row_editing_changed));


            //todo: break this down into individual notificaions so you dont have to update all 3 rows every time
            CardEditingChangedBroadcaseReceiver = new ActionBroadcastReceiver();
            CardEditingChangedBroadcaseReceiver.NotificationReceived += (obj) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    var replacementItems = Shared.GetTableItems();

                    var nameIndex = 1;
                    RecyclerViewAdapter.ListItems[nameIndex] = replacementItems[nameIndex];

                    var titleIndex = 2;
                    RecyclerViewAdapter.ListItems[titleIndex] = replacementItems[titleIndex];

                    var companyNameIndex = 5;
                    RecyclerViewAdapter.ListItems[companyNameIndex] = replacementItems[companyNameIndex];

                    RecyclerViewAdapter.NotifyItemRangeChanged(nameIndex, companyNameIndex);
                });
            };
            Activity.RegisterReceiver(CardEditingChangedBroadcaseReceiver, new IntentFilter(Strings.InternalNotifications.notification_card_editing_changed));


            NoOutletsTappedBroadcastReceiver = new ActionBroadcastReceiver();
            NoOutletsTappedBroadcastReceiver.NotificationReceived += (obj) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    RecyclerView.ScrollToPosition(RecyclerViewAdapter.ListItems.Count - 1);
                });
            };
            Activity.RegisterReceiver(NoOutletsTappedBroadcastReceiver, new IntentFilter(Strings.InternalNotifications.notification_no_outlets_tapped));

            CardUserImageClickedBroadcastReceiver = new ActionBroadcastReceiver();
            CardUserImageClickedBroadcastReceiver.NotificationReceived += (obj) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    var cell = RecyclerView.FindViewHolderForAdapterPosition(0) as CardCell;
                    if (cell == null) return;

                    var imgView = cell.GetUserImageView();
                    if (imgView == null) return;

                    ShowImageChooser(imgView, Shared.SelectedCard.LocalHeaderURL, Shared.SelectedCard.GetRemoteHeaderUrlCached(), "Profile.png", SelectUserImagePhotoRequestCode);
                });
            };
            Activity.RegisterReceiver(CardUserImageClickedBroadcastReceiver, new IntentFilter(Strings.InternalNotifications.notification_card_user_image_clicked));

            CardCompanyLogoImageClickedBroadcastReceiver = new ActionBroadcastReceiver();
            CardCompanyLogoImageClickedBroadcastReceiver.NotificationReceived += (obj) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    var cell = RecyclerView.FindViewHolderForAdapterPosition(0) as CardCell;
                    if (cell == null) return;

                    var imgView = cell.GetCompanyLogoImageView();
                    if (imgView == null) return;

                    ShowImageChooser(imgView, Shared.SelectedCard.LocalHeaderURL, Shared.SelectedCard.GetRemoteHeaderUrlCached(), "Logo.png", SelectCompanyLogoPhotoRequestCode);
                });
            };
            Activity.RegisterReceiver(CardCompanyLogoImageClickedBroadcastReceiver, new IntentFilter(Strings.InternalNotifications.notification_company_logo_image_clicked));

        }

        public override void OnStop()
        {
            base.OnStop();

            if (RowEditedBroadcastReceiver != null)
                Activity.UnregisterReceiver(RowEditedBroadcastReceiver);

            if (CardEditingChangedBroadcaseReceiver != null)
                Activity.UnregisterReceiver(CardEditingChangedBroadcaseReceiver);

            if (NoOutletsTappedBroadcastReceiver != null)
                Activity.UnregisterReceiver(NoOutletsTappedBroadcastReceiver);

            if (CardUserImageClickedBroadcastReceiver != null)
                Activity.UnregisterReceiver(CardUserImageClickedBroadcastReceiver);

            if (CardCompanyLogoImageClickedBroadcastReceiver != null)
                Activity.UnregisterReceiver(CardCompanyLogoImageClickedBroadcastReceiver);
        }
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            var resource = (Shared.SelectedCard == null) ? Resource.Menu.toolbar_save : Resource.Menu.toolbar_delete;
            inflater.Inflate(resource, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {


                case Resource.Id.Save:
                    SaveCardIfPossible();
                    break;

                case Resource.Id.Delete:
                    DeleteCard();
                    if (Shared.SelectedCard != null)
                    {
                        var convertedActiviy = (Activity as BaseActivity);
                        convertedActiviy.HideKeyboard();
                        convertedActiviy.PopFragmentOverUntil(typeof(MyCardsRecyclerViewFragment));
                         convertedActiviy.AddFragmentOver(new MyCardsRecyclerViewFragment());

                    }
                    else
                    {
                        var convertedActiviy = (Activity as BaseActivity);
                        convertedActiviy.HideKeyboard();
                        convertedActiviy.PopFragmentOver();
                    }
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        public override void RecyclerView_ItemClick(NewCardModel obj, int position)
        {
            base.RecyclerView_ItemClick(obj, position);

            //Footer
            if (obj == null)
            {
                var convertedActivity = Activity as BaseActivity;
                if (convertedActivity == null) return;

                var fragm = new MyOutletsRecyclerViewFragment();
                fragm.OutletSelected += (outlet) =>
                {
                    AddOutletToCard(outlet, true);
                };
                ((MainActivity)Activity).AddFragmentOver(fragm);
                return;
            }

            if (String.IsNullOrEmpty(obj.Title)) return;
            if (obj.Editable) return;

            (Activity as BaseActivity)?.HideKeyboard();

            var frag = new ColorPickerAppCompatActivity();
            frag.ColorPicked += (color) =>
            {
                if (obj == null && String.IsNullOrEmpty(obj.Title)) return;

                obj.ColorHexString = ColorUtils.HexStringFromColor(color);

                if (obj.Title.Equals(NewCardShared.new_card_model_border_color, StringComparison.InvariantCulture))
                {
                    Shared.SelectedCard.UpdateStringProperty(() => Shared.SelectedCard.BorderColor, obj.ColorHexString);
                    Shared.SelectedCard.ShowFront();
                    return;
                }

                if (obj.Title.Equals(NewCardShared.new_card_model_background_color, StringComparison.InvariantCulture))
                {
                    Shared.SelectedCard.UpdateStringProperty(() => Shared.SelectedCard.BackgroundColor, obj.ColorHexString);
                    Shared.SelectedCard.ShowBack();
                    return;
                }

                if (obj.Title.Equals(NewCardShared.new_card_model_company_name_text_color, StringComparison.InvariantCulture))
                {
                    Shared.SelectedCard.UpdateStringProperty(() => Shared.SelectedCard.CompanyNameTextColor, obj.ColorHexString);
                    Shared.SelectedCard.ShowBack();
                    return;
                }

                RecyclerViewAdapter.NotifyDataSetChanged();

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
        public override bool OnContextItemSelected(IMenuItem item)
        {
            var position = item.Order;

            var model = RecyclerViewAdapter.GetItemInList(position);
            if (model == null) return base.OnContextItemSelected(item);
            if (model.Outlet == null) return base.OnContextItemSelected(item);

            switch (item.TitleFormatted.ToString().ToLower())
            {
                case "delete":
                    Shared.SelectedCard.RemoveOutlet(model.Outlet);

                    var replacementItems = Shared.GetTableItems();
                    RecyclerViewAdapter.ListItems.RemoveAt(position);

                    RecyclerViewAdapter.NotifyItemRemoved(position);
                    RecyclerViewAdapter.NotifyItemChanged(0);
                    break;
                default: break;
            }

            return base.OnContextItemSelected(item);
        }
        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == (int)Android.App.Result.Ok)
            {
                if (data == null) return;

                using (var bitmap = MediaStore.Images.Media.GetBitmap(Activity.ContentResolver, data.Data))
                {
                    var bytes = ImageUtils.ImagetoByteArray(bitmap, 100);
                    if (bytes == null) return;

                    var cell = RecyclerView.FindViewHolderForAdapterPosition(0) as CardCell;
                    if (cell == null) return;

                    var me = RealmUserServices.GetMe(false);

                    if (requestCode == SelectUserImagePhotoRequestCode)
                    {
                        var imageView = cell.GetUserImageView();
                        imageView.SetImageResource(Resource.Drawable.ic_noprofilewhite);
                        imageView.ShowLoadingIndicators();


                        var localUrl = Shared.SelectedCard.LocalHeaderURL;
                        var remoteUrl = Shared.SelectedCard.RemoteHeaderURL;
                        var fileName = "Header.png";

                        S3Utils.UploadPhoto(bytes, localUrl, remoteUrl, fileName, () =>
                        {
                            if (Activity == null) return;
                            Activity.RunOnUiThread(async () =>
                            {
                                await ImageService.Instance.InvalidateCacheAsync(FFImageLoading.Cache.CacheType.All);//.InvalidateCacheEntryAsync(me.RemoteProfileImageURL, FFImageLoading.Cache.CacheType.All);
                                imageView.SetImage(Shared.SelectedCard.GetRemoteHeaderUrlCached(), Resource.Drawable.ic_noprofilewhite, Resource.Drawable.ic_noprofilewhite, Shared.SelectedCard.RemoteHeaderURL, WebImageView.DefaultCircleTransformation);
                            });
                        }, null);

                        return;
                    }

                    if (requestCode == SelectCompanyLogoPhotoRequestCode)
                    {
                        var imageView = cell.GetCompanyLogoImageView();
                        imageView.SetImageResource(Resource.Drawable.ic_buildings);
                        imageView.ShowLoadingIndicators();

                        var localUrl = Shared.SelectedCard.LocalLogoURL;
                        var remoteUrl = Shared.SelectedCard.RemoteLogoURL;
                        var fileName = "Logo.png";

                        S3Utils.UploadPhoto(bytes, localUrl, remoteUrl, fileName, () =>
                        {
                            if (Activity == null) return;
                            Activity.RunOnUiThread(async () =>
                            {
                                await ImageService.Instance.InvalidateCacheAsync(FFImageLoading.Cache.CacheType.All);//.InvalidateCacheEntryAsync(me.RemoteProfileImageURL, FFImageLoading.Cache.CacheType.All);
                                imageView.SetImage(Shared.SelectedCard.GetRemoteLogoUrlCached(), Resource.Drawable.ic_buildings, Resource.Drawable.ic_buildings, Shared.SelectedCard.RemoteLogoURL, WebImageView.DefaultCircleTransformation);
                            });
                        }, null);

                        return;

                    }
                };
            }
        }


        public bool SaveCardIfPossible()
        {
            var card = Shared.SelectedCard;
            var name = card.Name;
            if (String.IsNullOrEmpty(name) || name.Equals(Strings.Basic.new_card, StringComparison.InvariantCultureIgnoreCase))
            {
                ShowCardMissingNameAlert();
                return false;
            }

            if (card.Outlets.Count == 0)
            {
                ShowCardMissingOutletsAlert();
                return false;
            }

            var me = RealmUserServices.GetMe(false);
            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                card.Owner = me;
            });


            return true;
        }
        void ShowCardMissingNameAlert()
        {
            var builder = new AlertDialog.Builder(Activity);
            builder.SetTitle(Strings.Alerts.card_missing_name);
            builder.SetCancelable(true);

            builder.SetPositiveButton(Strings.Basic.ok, (senderAlert, args) =>
            {
                var cell = RecyclerView.FindViewHolderForAdapterPosition(0) as CardCell;
                cell.FocusOnName();
            });
            builder.SetNegativeButton(Strings.Basic.delete_card, delegate
            {
                var activity = Activity as BaseActivity;
                activity?.HideKeyboard();
                activity?.PopFragmentOver();
            });
            builder.Show();

        }
        void ShowCardMissingOutletsAlert()
        {
            var builder = new AlertDialog.Builder(Activity);
            builder.SetTitle(Strings.Alerts.card_missing_outlets);
            builder.SetCancelable(true);

            builder.SetPositiveButton(Strings.Basic.ok, (senderAlert, args) =>
            {
                RecyclerView.ScrollToPosition(RecyclerViewAdapter.ListItems.Count - 1);
            });
            builder.SetNegativeButton(Strings.Basic.delete_card, delegate
            {
                var activity = Activity as BaseActivity;
                activity?.HideKeyboard();
                activity?.PopFragmentOver();
            });
            builder.Show();
        }

        void DeleteCard()
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                Shared.SelectedCard.Deleted = true;
            });
        }
        void AddOutletToCard(Outlet obj, bool dismiss)
        {
            Shared.SelectedCard.AddOutlet(obj);

            RecyclerViewAdapter.SetListItems(Shared.GetTableItems());
            IfEmpty(true);

            if (dismiss)
            {
                var convertedActiviy = (Activity as BaseActivity);
                convertedActiviy.PopFragmentOver();
            }

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

        private void BlurTheView(View view)
        {
            RenderScript rs = RenderScript.Create(this.Activity);
            var bitmap = GetBitmapFromView(view);
            var blurprocesor = new RSBlurProcessor(rs);
            var blurBitmap = blurprocesor.blur(bitmap, 15, 1);
            view.Background = new BitmapDrawable(blurBitmap);
        }



        public Bitmap GetBitmapFromView(View view)
        {
            DisplayMetrics displayMetrics = new DisplayMetrics();
            this.Activity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            int height = displayMetrics.HeightPixels;
            int width = displayMetrics.WidthPixels;
            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas c = new Canvas(bitmap);
            view.Layout(view.Left, view.Top, view.Right, view.Bottom);
            view.Draw(c);
            return bitmap;
        }

    }
}
