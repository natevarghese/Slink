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
                ShowImageChooser(UserProfileImage, me.LocalProfileImageURL, me.RemoteProfileImageURL, "Profile.png", SelectUserImagePhotoRequestCode);
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

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == SelectUserImagePhotoRequestCode && resultCode == (int)Android.App.Result.Ok)
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
                FirstNameEditText.SetInvalid(Resources);
                allFieldsValid = false;
            }


            if (String.IsNullOrEmpty(lastName))
            {
                LastNameEditText.SetInvalid(Resources);
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
