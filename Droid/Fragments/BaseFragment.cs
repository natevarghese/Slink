using System;
using Android.App;
using Android.Content;
using Android.Support.V7.App;
using FFImageLoading;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Slink.Droid
{
    public class BaseFragment : Android.Support.V4.App.Fragment
    {
        ProgressDialog Progress;

        public int SelectUserImagePhotoRequestCode = 100;
        public int SelectCompanyLogoPhotoRequestCode = 101;

        public virtual void ShowHud(string str)
        {
            if (Progress == null)
            {
                Progress = new ProgressDialog(Activity);
                Progress.Indeterminate = true;
                Progress.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
                Progress.SetMessage(str);
                Progress.SetCancelable(false);
            }
            Progress.Show();
        }

        public virtual void HideHud()
        {
            Progress?.Dismiss();
            Progress = null;
        }

        public void SetOverFragment(string className)
        {
            var intent = new Intent(LandingActivity.UpdateOverFragmentBroadcastReceiverKey);
            intent.PutExtra(LandingActivity.UpdateOverFragmentBroadcastReceiverKeyFileName, className);
            Context.SendBroadcast(intent);
        }



        public void ShowImageChooser(WebImageView imageView, string localUrl, string remoteUrl, string fileName, int requestCode)
        {
            var activity = Activity as MainActivity;
            activity?.HideKeyboard();

            var me = RealmUserServices.GetMe(false);

            var builder = new Android.Support.V7.App.AlertDialog.Builder(Activity);
            builder.SetTitle(Strings.Alerts.select_image_source);
            builder.SetCancelable(true);

            builder.SetPositiveButton(Strings.Alerts.user_facebook_image, (senderAlert, args) =>
            {
                DownloadFacebookImage(imageView, localUrl, remoteUrl, fileName);
            });
            builder.SetNegativeButton(Strings.Alerts.select_from_gallery, async delegate
            {
                var storagePermission = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (storagePermission == PermissionStatus.Granted)
                {
                    SelectImageFromGallery(imageView, localUrl, remoteUrl, fileName, requestCode);
                }
                else
                {
                    var dict = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    if (dict.ContainsKey(Permission.Storage) && dict[Permission.Storage] == PermissionStatus.Granted)
                    {
                        SelectImageFromGallery(imageView, localUrl, remoteUrl, fileName, requestCode);
                    }
                    else
                    {
                        //alert
                    }
                }
            });
            builder.Show();
        }
        public async void DownloadFacebookImage(WebImageView imageView, string localUrl, string remoteUrl, string fileName)
        {
            if (imageView == null) return;

            var me = RealmUserServices.GetMe(false);
            if (me == null) return;

            var url = me.GetFacebookProfilePictureUrl();
            if (url == null) return;

            imageView.SetImageResource(Resource.Drawable.ic_noprofilewhite);

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
                        imageView.SetImage(me.GetRemoteProfileImageUrlCached(), Resource.Drawable.ic_noprofilewhite, Resource.Drawable.ic_noprofilewhite, me.RemoteProfileImageURL, WebImageView.DefaultCircleTransformation);
                    });
                }, null);
            })
            .Finish((FFImageLoading.Work.IScheduledWork obj) =>
            {
            })
            .Transform(WebImageView.DefaultCircleTransformation)
            .Error(exception =>
            {
                imageView.ShowLoadingIndicators();
                return;
            })
            .Into(imageView);
        }
        public void SelectImageFromGallery(WebImageView imageView, string localUrl, string remoteUrl, string fileName, int requestCode)
        {
            if (imageView == null) return;


            var intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Select Photo"), requestCode);
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Progress?.Dispose();
                Progress = null;
            }
        }
    }
}
