using System;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Slink.Droid
{
    public class EditProfileFragment : BaseFragment
    {
        EditText FirstNameEditText, LastNameEditText;
        WebImageView UserProfileImage;

        int SelectPhotoRequestCode = 100;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var me = RealmUserServices.GetMe(false);

            var view = inflater.Inflate(Resource.Layout.EditProfile, container, false);

            FirstNameEditText = view.FindViewById<EditText>(Resource.Id.FirstNameEditText);
            FirstNameEditText.Text = me.FirstName;
            FirstNameEditText.Hint = Strings.Basic.first_name;

            LastNameEditText = view.FindViewById<EditText>(Resource.Id.LastNameEditText);
            LastNameEditText.Text = me.LastName;
            LastNameEditText.Hint = Strings.Basic.last_name;

            UserProfileImage = view.FindViewById<WebImageView>(Resource.Id.WebImageView);
            UserProfileImage.SetImage(me.GetRemoteProfileImageUrlCached(), Resource.Drawable.ic_noprofilewhite, Resource.Drawable.ic_noprofilewhite, me.RemoteProfileImageURL, WebImageView.DefaultCircleTransformation);
            UserProfileImage.Click += (sender, e) =>
            {
                var activity = Activity as MainActivity;
                activity?.HideKeyboard();

                var builder = new AlertDialog.Builder(Activity);
                builder.SetTitle(Strings.Alerts.select_image_source);
                builder.SetCancelable(true);

                builder.SetPositiveButton(Strings.Alerts.user_facebook_image, (senderAlert, args) =>
                {
                    DownloadFacebookImage(me.LocalProfileImageURL, me.RemoteProfileImageURL, "Profile.png");
                });
                builder.SetNegativeButton(Strings.Alerts.select_from_gallery, async delegate
                {
                    var storagePermission = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                    if (storagePermission == PermissionStatus.Granted)
                    {
                        SelectImageFromGallery(me.LocalProfileImageURL, me.RemoteProfileImageURL, "Profile.png");
                    }
                    else
                    {
                        var dict = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                        if (dict.ContainsKey(Permission.Storage) && dict[Permission.Storage] == PermissionStatus.Granted)
                        {
                            SelectImageFromGallery(me.LocalProfileImageURL, me.RemoteProfileImageURL, "Profile.png");
                        }
                        else
                        {
                            //alert
                        }
                    }
                });
                builder.Show();
            };


            var progressButton = view.FindViewById<Button>(Resource.Id.ProgressButton);
            progressButton.Click += (object sender, EventArgs e) =>
            {
                string firstName = FirstNameEditText.Text.Trim();
                string lastName = LastNameEditText.Text.Trim();

                bool allFieldsValid = ValidateAllFields();
                if (!allFieldsValid) return;

                me.UpdateStringProperty(() => me.FirstName, firstName);
                me.UpdateStringProperty(() => me.LastName, lastName);

                DismissIfValid();
            };

            return view;
        }
        async void DownloadFacebookImage(string localUrl, string remoteUrl, string fileName)
        {
            if (UserProfileImage == null) return;

            var me = RealmUserServices.GetMe(false);
            if (me == null) return;

            var url = me.GetFacebookProfilePictureUrl();
            if (url == null) return;

            UserProfileImage.SetImageResource(Resource.Drawable.ic_noprofilewhite);

            //required to remove it here otherwise itll load form cache
            await ImageService.Instance.InvalidateCacheEntryAsync(me.RemoteProfileImageURL, FFImageLoading.Cache.CacheType.All, true);

            ImageService.Instance.LoadUrl(url).Success(async (FFImageLoading.Work.ImageInformation arg1, FFImageLoading.Work.LoadingResult arg2) =>
            {
                if (arg1 == null) return;

                var image = await ImageUtils.GetImageAtPath(arg1.FilePath);
                if (image == null) return;

                var bytes = ImageUtils.ImagetoByteArray(image, 100);
                if (bytes == null) return;

                S3Utils.UploadPhoto(bytes, localUrl, remoteUrl, fileName, () =>
                {
                    if (Activity == null) return;
                    Activity.RunOnUiThread(async () =>
                    {
                        await ImageService.Instance.InvalidateCacheAsync(FFImageLoading.Cache.CacheType.All);//.InvalidateCacheEntryAsync(me.RemoteProfileImageURL, FFImageLoading.Cache.CacheType.All);
                        UserProfileImage.SetImage(me.GetRemoteProfileImageUrlCached(), Resource.Drawable.ic_noprofilewhite, Resource.Drawable.ic_noprofilewhite, me.RemoteProfileImageURL, WebImageView.DefaultCircleTransformation);
                    });
                }, null);
            })
            .Finish((FFImageLoading.Work.IScheduledWork obj) =>
            {
            })
            .Transform(WebImageView.DefaultCircleTransformation)
            .Error(exception =>
            {
                UserProfileImage.ShowLoadingIndicators();
                return;
            })
            .Into(UserProfileImage);
        }
        void SelectImageFromGallery(string localUrl, string remoteUrl, string fileName)
        {
            if (UserProfileImage == null) return;


            var intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Select Photo"), SelectPhotoRequestCode);
        }
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == SelectPhotoRequestCode && resultCode == (int)Android.App.Result.Ok)
            {
                if (data == null) return;

                using (var bitmap = MediaStore.Images.Media.GetBitmap(Activity.ContentResolver, data.Data))
                {
                    var bytes = ImageUtils.ImagetoByteArray(bitmap, 100);
                    if (bytes == null) return;

                    UserProfileImage.SetImageResource(Resource.Drawable.ic_noprofilewhite);
                    UserProfileImage.ShowLoadingIndicators();

                    var me = RealmUserServices.GetMe(false);
                    var localUrl = me.LocalProfileImageURL;
                    var remoteUrl = me.RemoteProfileImageURL;
                    var fileName = "Profile.png";

                    S3Utils.UploadPhoto(bytes, localUrl, remoteUrl, fileName, () =>
                    {
                        if (Activity == null) return;
                        Activity.RunOnUiThread(async () =>
                        {
                            await ImageService.Instance.InvalidateCacheAsync(FFImageLoading.Cache.CacheType.All);//.InvalidateCacheEntryAsync(me.RemoteProfileImageURL, FFImageLoading.Cache.CacheType.All);
                            UserProfileImage.SetImage(me.GetRemoteProfileImageUrlCached(), Resource.Drawable.ic_noprofilewhite, Resource.Drawable.ic_noprofilewhite, me.RemoteProfileImageURL, WebImageView.DefaultCircleTransformation);
                        });
                    }, null);
                };
            }
        }
        bool ValidateAllFields()
        {
            string firstName = FirstNameEditText.Text.Trim();
            string lastName = LastNameEditText.Text.Trim();

            bool allFieldsValid = true;

            if (String.IsNullOrEmpty(firstName))
            {
                //FirstNameEditText.SetInvalid();
                allFieldsValid = false;
            }


            if (String.IsNullOrEmpty(lastName))
            {
                //LastNameEditText.SetInvalid();
                allFieldsValid = false;
            }

            return allFieldsValid;
        }
        void DismissIfValid()
        {
            if (!ValidateAllFields()) return;

            Activity.OnBackPressed();
        }
    }
}
