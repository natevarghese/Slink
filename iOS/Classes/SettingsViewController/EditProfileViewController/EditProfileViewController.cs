using System;
using CoreGraphics;
using Foundation;
using Plugin.DeviceInfo;
using Plugin.DeviceInfo.Abstractions;
using SDWebImage;
using UIKit;

namespace Slink.iOS
{
    public partial class EditProfileViewController : BaseLandingViewController
    {
        string FallbackImageFileName = "NoProfileWhite";

        public EditProfileViewController(IntPtr handle) : base(handle) { }
        public EditProfileViewController() : base("EditProfileViewController") { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var me = RealmUserServices.GetMe(false);

            FirstNameTextField.KeyboardType = UIKeyboardType.EmailAddress;
            FirstNameTextField.Text = me.FirstName;
            FirstNameTextField.Placeholder = Strings.Basic.first_name;
            FirstNameTextField.AutocorrectionType = UITextAutocorrectionType.No;
            FirstNameTextField.EditingDidEndOnExit += (sender, e) =>
            {
                FirstNameTextField.ResignFirstResponder();
            };

            LastNameTextField.KeyboardType = UIKeyboardType.EmailAddress;
            LastNameTextField.Text = me.LastName;
            LastNameTextField.Placeholder = Strings.Basic.last_name;
            LastNameTextField.AutocorrectionType = UITextAutocorrectionType.No;
            LastNameTextField.EditingDidEndOnExit += (sender, e) =>
            {
                LastNameTextField.ResignFirstResponder();
            };

            ProfileImageButton.SetImageWithCustomCache(me.GetRemoteProfileImageUrlCached(), FallbackImageFileName, FallbackImageFileName, me.RemoteProfileImageURL);
            ProfileImageButton.Layer.MasksToBounds = true;
            ProfileImageButton.Layer.CornerRadius = ProfileImageButton.Frame.Size.Width / 2;
            ProfileImageButton.ClipsToBounds = true;
            ProfileImageButton.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;

            SetProgressButton(SaveButton);

            RemoveBackBarButtonTitle();
        }


        protected override bool ValidateAllFields()
        {
            string firstName = FirstNameTextField.Text.Trim();
            string lastName = LastNameTextField.Text.Trim();

            bool allFieldsValid = true;

            if (String.IsNullOrEmpty(firstName))
            {
                FirstNameTextField.SetInvalid();
                allFieldsValid = false;
            }


            if (String.IsNullOrEmpty(lastName))
            {
                LastNameTextField.SetInvalid();
                allFieldsValid = false;
            }

            return allFieldsValid;
        }

        public override TPKeyboardAvoiding.TPKeyboardAvoidingScrollView GetScrollView()
        {
            return ScrollView;
        }

        partial void ProfileImageButtonClicked(Foundation.NSObject sender)
        {
            var isTablet = CrossDeviceInfo.Current.Idiom == Idiom.Tablet;
            var alertStyle = isTablet ? UIAlertControllerStyle.Alert : UIAlertControllerStyle.ActionSheet;

            UIAlertController AlertController = UIAlertController.Create(Strings.Alerts.select_image_source, null, alertStyle);
            AlertController.AddAction(UIAlertAction.Create(Strings.Alerts.user_facebook_image, UIAlertActionStyle.Default, (obj) =>
            {
                var me = RealmUserServices.GetMe(false);
                DownloadFacebookImage(me.LocalProfileImageURL, me.RemoteProfileImageURL, "Profile.png");
            }));
            AlertController.AddAction(UIAlertAction.Create(Strings.Alerts.select_from_gallery, UIAlertActionStyle.Default, (obj) =>
            {
                var me = RealmUserServices.GetMe(false);
                SelectImageFromGallery(me.LocalProfileImageURL, me.RemoteProfileImageURL, "Profile.png");
            }));
            AlertController.AddAction(UIAlertAction.Create(Strings.Basic.cancel, UIAlertActionStyle.Cancel, null));
            PresentViewController(AlertController, true, null);
        }
        void DownloadFacebookImage(string localUrl, string remoteUrl, string fileName)
        {
            if (ProfileImageButton == null) return;

            var me = RealmUserServices.GetMe(false);
            if (me == null) return;

            var url = me.GetFacebookProfilePictureUrl();
            if (url == null) return;

            ProfileImageButton.SetImage(UIImage.FromBundle(FallbackImageFileName), new UIControlState());
            ProfileImageButton.ShowLoadingIndicators();
            SDWebImageManager.SharedManager.ImageDownloader.DownloadImage(NSUrl.FromString(url), SDWebImageDownloaderOptions.HighPriority, null, (image, data, error, finished) =>
            {
                if (image == null || error != null)
                {
                    ProfileImageButton.ShowLoadingIndicators();
                    return;
                }

                var bytes = ImageUtils.ByteArrayFromImage(image, 100);
                S3Utils.UploadPhoto(bytes, localUrl, remoteUrl, fileName, () =>
                {

                    SDWebImageManager.SharedManager.ImageCache.RemoveImage(me.RemoteProfileImageURL, true, null);
                    ProfileImageButton.SetImageWithCustomCache(me.GetRemoteProfileImageUrlCached(), FallbackImageFileName, FallbackImageFileName, me.RemoteProfileImageURL);
                }, null);
            });
        }
        void SelectImageFromGallery(string localUrl, string remoteUrl, string fileName)
        {
            if (ProfileImageButton == null) return;

            var vc = new GalleryImagePicker();
            vc.Canceled += (s, e) => { vc.DismissViewController(true, null); };
            vc.FinishedPickingMedia += (object s, UIImagePickerMediaPickedEventArgs e) =>
            {
                switch (e.Info[UIImagePickerController.MediaType].ToString())
                {
                    case "public.image":
                        Console.WriteLine("Image selected");

                        var me = RealmUserServices.GetMe(false);

                        ProfileImageButton.SetImage(UIImage.FromBundle(FallbackImageFileName), new UIControlState());
                        ProfileImageButton.ShowLoadingIndicators();

                        UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
                        var smallerImage = ImageUtils.ScaledToSize(originalImage, new CGSize(200, 200));
                        var bytes = ImageUtils.ByteArrayFromImage(smallerImage, 100);
                        S3Utils.UploadPhoto(bytes, localUrl, remoteUrl, fileName, () =>
                        {
                            SDWebImageManager.SharedManager.ImageCache.RemoveImage(me.RemoteProfileImageURL, true, null);
                            ProfileImageButton.SetImageWithCustomCache(me.GetRemoteProfileImageUrlCached(), FallbackImageFileName, FallbackImageFileName, me.RemoteProfileImageURL);
                        }, null);


                        break;
                    case "public.video":
                        Console.WriteLine("Video selected");
                        break;
                }
                vc.DismissViewController(true, null);
            };
            PresentViewController(vc, true, null);
        }
        partial void SaveButtonClicked(Foundation.NSObject sender)
        {
            View.EndEditing(true);

            string firstName = FirstNameTextField.Text.Trim();
            string lastName = LastNameTextField.Text.Trim();

            bool allFieldsValid = ValidateAllFields();
            if (!allFieldsValid) return;

            var me = RealmUserServices.GetMe(false);
            me.UpdateStringProperty(() => me.FirstName, firstName);
            me.UpdateStringProperty(() => me.LastName, lastName);

            DismissIfValid();
        }

        void DismissIfValid()
        {
            if (!ValidateAllFields()) return;

            NavigationController.PopViewController(true);
        }
    }
}