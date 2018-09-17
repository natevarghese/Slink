using System; using UIKit; using CoreGraphics; using System.Linq; using CoreAnimation; using Foundation; using SDWebImage;
using Plugin.DeviceInfo; using Plugin.DeviceInfo.Abstractions; 
namespace Slink.iOS
{
    public partial class CardViewController : BaseViewController
    {
        public Card SelectedCard;
        public bool Editable, HideTitle;

        NSObject TableViewRowEditingChangedNotification;

        public CardViewController() : base("CardViewController") { }

        public override void ViewDidLoad()
        {             NetworkListenerEnabled = false; 
            base.ViewDidLoad();
             NameTextField.Enabled = Editable;             NameTextField.AttributedPlaceholder = new NSAttributedString(Strings.Basic.new_card, new UIStringAttributes { ForegroundColor = UIColor.Gray });             NameTextField.EditingChanged += (sender, e) =>             {                 SelectedCard.UpdateStringProperty(() => SelectedCard.Name, NameTextField.Text.Trim());             };             NameTextField.Hidden = HideTitle; 
            var cardBack = CardBack.Create(); 
            cardBack.Hidden = true;
            cardBack.Frame = ContainerView.Bounds;
            cardBack.LogoImageButtonAction += () =>
            {                 var alertStyle =  UIAlertControllerStyle.Alert;                  UIAlertController AlertController = UIAlertController.Create(Strings.Alerts.select_image_source, null, alertStyle);                 AlertController.AddAction(UIAlertAction.Create(Strings.Alerts.user_facebook_image, UIAlertActionStyle.Default, (obj) =>                 {                     DownloadFacebookImage(cardBack, SelectedCard.LocalLogoURL, SelectedCard.RemoteLogoURL, "Logo.png", SelectedCard.RemoteLogoURL);                 }));                 AlertController.AddAction(UIAlertAction.Create(Strings.Alerts.select_from_gallery, UIAlertActionStyle.Default, (obj) =>                 {                     SelectImageFromGallery(cardBack, SelectedCard.LocalLogoURL, SelectedCard.RemoteLogoURL, "Logo.png",SelectedCard.RemoteLogoURL);                 }));                 AlertController.AddAction(UIAlertAction.Create(Strings.Basic.cancel, UIAlertActionStyle.Cancel, null));                 PresentViewController(AlertController, true, null);
            };
            ContainerView.AddSubview(cardBack);
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardBack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Top, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardBack, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Right, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardBack, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Bottom, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardBack, NSLayoutAttribute.Left, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Left, 1, 0));

            var cardFront = CardFront.Create();
            cardFront.Hidden = true;
            cardFront.Frame = ContainerView.Bounds;
            cardFront.HeaderImageButtonAction += () =>
            {                 var alertStyle = UIAlertControllerStyle.Alert;                  UIAlertController AlertController = UIAlertController.Create(Strings.Alerts.select_image_source, null, alertStyle);                 AlertController.AddAction(UIAlertAction.Create(Strings.Alerts.user_facebook_image, UIAlertActionStyle.Default, (obj) =>                 {                     DownloadFacebookImage(cardFront, SelectedCard.LocalHeaderURL, SelectedCard.RemoteHeaderURL, "Header.png", SelectedCard.RemoteHeaderURL);                 }));                 AlertController.AddAction(UIAlertAction.Create(Strings.Alerts.select_from_gallery, UIAlertActionStyle.Default, (obj) =>                 {                     SelectImageFromGallery(cardFront, SelectedCard.LocalHeaderURL, SelectedCard.RemoteHeaderURL, "Header.png", SelectedCard.RemoteHeaderURL);                 }));                 AlertController.AddAction(UIAlertAction.Create(Strings.Basic.cancel, UIAlertActionStyle.Cancel, null));                 PresentViewController(AlertController, true, null);
            };
            ContainerView.AddSubview(cardFront);
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardFront, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Top, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardFront, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Right, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardFront, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Bottom, 1, 0));
            ContainerView.AddConstraint(NSLayoutConstraint.Create(cardFront, NSLayoutAttribute.Left, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Left, 1, 0));
             if (SelectedCard.IsFlipped)             {                 cardBack.Hidden = false;             }             else             {                 cardFront.Hidden = false;             }               var SwipeGestureRecognizer = new UISwipeGestureRecognizer((UISwipeGestureRecognizer obj) =>             {                 Flip();             });             SwipeGestureRecognizer.Direction = UISwipeGestureRecognizerDirection.Left | UISwipeGestureRecognizerDirection.Right;             ContainerView.AddGestureRecognizer(SwipeGestureRecognizer);           }          public override void ViewWillAppear(bool animated)         {             base.ViewWillAppear(animated);              Update(true);              CABasicAnimation grow = CABasicAnimation.FromKeyPath("transform.scale");             grow.From = NSObject.FromObject(0);             grow.Duration = .5;             grow.RemovedOnCompletion = true;             ContainerView.Layer.AddAnimation(grow, "grow");         }         public override void ViewDidAppear(bool animated)         {             base.ViewDidAppear(animated);              TableViewRowEditingChangedNotification = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(Strings.InternalNotifications.notification_table_row_editing_changed), (obj) =>             {                 InvokeOnMainThread(() =>                 {                     Update(false);                 });             });         }         public override void ViewDidDisappear(bool animated)         {             base.ViewDidDisappear(animated);              TableViewRowEditingChangedNotification?.Dispose();             TableViewRowEditingChangedNotification = null;         }         public override void ViewWillLayoutSubviews()         {             base.ViewWillLayoutSubviews();              if (ContainerView != null)             {                 ContainerView.Frame = new CGRect(8, 8, View.Bounds.Width - 16, GetCalculatedHeight() + 33); //40 is the total insets that appear in a tableviewcell                  var cardFront = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardFront)).FirstOrDefault() as CardFront;                 if (cardFront != null)                     cardFront.Frame = ContainerView.Bounds;                  var cardBack = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardBack)).FirstOrDefault() as CardBack;                 if (cardBack != null)                      cardBack.Frame = ContainerView.Bounds;             }         }         protected override void Dispose(bool disposing)         {             base.Dispose(disposing);             if (disposing)             {                 ContainerView.Subviews.ToList().ForEach(v => v.RemoveFromSuperview());                 ContainerView?.RemoveFromSuperview();                 ContainerView?.Dispose();                 ContainerView = null;                  TableViewRowEditingChangedNotification?.Dispose();                 TableViewRowEditingChangedNotification = null;             }         }         public void FocusOnName()         {             NameTextField.BecomeFirstResponder();         }         public void Update(bool reloadImages)         {             if (SelectedCard == null) return;              NameTextField.Text = (SelectedCard.Name.Equals(Strings.Basic.new_card, StringComparison.InvariantCultureIgnoreCase) && Editable) ? null : SelectedCard.Name;              var cardFront = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardFront)).FirstOrDefault() as CardFront;
			if (cardFront != null)                 cardFront.BindDataToView(SelectedCard, Editable, null, reloadImages);              var cardBack = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardBack)).FirstOrDefault() as CardBack;
			if (cardBack != null)                  cardBack.BindDataToView(SelectedCard, Editable);              PerformFlipAnimationIfNecessary();         }         public void PerformFlipAnimationIfNecessary()         {             var cardFront = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardFront)).First();             var cardBack = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardBack)).First();              //animation not needed             if (cardFront.Hidden == SelectedCard.IsFlipped) return;              UIView.Transition(View, 0.5, UIViewAnimationOptions.TransitionFlipFromRight, () =>             {                 cardFront.Hidden = SelectedCard.IsFlipped;                 cardBack.Hidden = !SelectedCard.IsFlipped;             }, null);         }          public void Flip()         {             if (SelectedCard == null) return;              SelectedCard.Flip();              var cardFront = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardFront)).First();             var cardBack = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardBack)).First();              UIView.Transition(View, 0.5, UIViewAnimationOptions.TransitionFlipFromRight, () =>             {                 cardFront.Hidden = SelectedCard.IsFlipped;                 cardBack.Hidden = !SelectedCard.IsFlipped;             }, null);         }          void DownloadFacebookImage(ICardView target, string localUrl, string remoteUrl, string fileName, string cacheKey)
        {             if (target == null) return;              var me = RealmUserServices.GetMe(false);             if (me == null) return;              var url = me.GetFacebookProfilePictureUrl();             if (url == null) return;                                         target.ToggleLoadingIndicators(true);             SDWebImageManager.SharedManager.ImageDownloader.DownloadImage(NSUrl.FromString(url), SDWebImageDownloaderOptions.HighPriority, null, (image, data, error, finished) =>              {                 if (image == null || error != null)                 {                     target.ToggleLoadingIndicators(false);                     return;                 }                  var bytes = ImageUtils.ByteArrayFromImage(image, 100);                 S3Utils.UploadPhoto(bytes, localUrl,remoteUrl, fileName, () =>                 {                     SDWebImageManager.SharedManager.ImageCache.RemoveImage(cacheKey, true, null);                     Update(true);                 }, null);             });         }
        void SelectImageFromGallery(ICardView target, string localUrl, string remoteUrl, string fileName,string cacheKey)
        {             if (target == null) return;              var vc = new GalleryImagePicker();             vc.Canceled += (sender, e) => { vc.DismissViewController(true, null); };             vc.FinishedPickingMedia += (object sender, UIImagePickerMediaPickedEventArgs e) =>             {                 switch (e.Info[UIImagePickerController.MediaType].ToString())                 {                     case "public.image":                         Console.WriteLine("Image selected");                          target.ToggleLoadingIndicators(true);                         UIImage editedImage = e.Info[UIImagePickerController.EditedImage] as UIImage;                         editedImage = ImageUtils.ScaledToSize(editedImage, new CGSize(200, 200));                         var bytes = ImageUtils.ByteArrayFromImage(editedImage, 100);                         S3Utils.UploadPhoto(bytes,localUrl, remoteUrl, fileName, () =>                         {                             SDWebImageManager.SharedManager.ImageCache.ClearDisk();                             SDWebImageManager.SharedManager.ImageCache.ClearMemory();                             Update(true);                         }, null);                           break;                     case "public.video":                         Console.WriteLine("Video selected");                         break;                 }                 vc.DismissViewController(true, null);             };             PresentViewController(vc, true, null);         }


        #region Used For Animation         public void AnimateTextNameLabel(string text, double delay, Action completed)         {             NameTextField.AnimateText(text, delay, completed);         }
		public void AnimateTextUserNameLabel(string text, double delay, Action completed)
		{
			var cardFront = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardFront)).FirstOrDefault() as CardFront;
            if (cardFront != null)
                cardFront.AnimateTextUserNameLabel(text, delay, completed);
		}
		public void AddUserImage(string url)
		{
			var cardFront = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardFront)).FirstOrDefault() as CardFront;
			if (cardFront != null)
                cardFront.AddUserImage(url);
		}
		public void ChangeBorderColor(UIColor color)
		{
			var cardFront = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardFront)).FirstOrDefault() as CardFront;
			if (cardFront != null)
                cardFront.ChangeBorderColor(color);
		}
		public void AddEmptyPhoneNumber()
		{
			var cardFront = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardFront)).FirstOrDefault() as CardFront;
            if (cardFront != null)
            {                 var phone = new Outlet();
				phone.Type = Outlet.outlet_type_phone;
                SelectedCard.Outlets.Add(phone); 
                cardFront.AddEmptyPhoneNumber();
            }
		}         public void AddEmptySocialMedia()         {
			var cardFront = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardFront)).FirstOrDefault() as CardFront;
			if (cardFront != null)
			{
                var facebook = new Outlet();
                facebook.Type = Outlet.outlet_type_facebook;
				SelectedCard.Outlets.Add(facebook);

				var twitter = new Outlet();
                twitter.Type = Outlet.outlet_type_twitter;
				SelectedCard.Outlets.Add(twitter);

				var instagram = new Outlet();
                instagram.Type = Outlet.outlet_type_instagram;
				SelectedCard.Outlets.Add(instagram);

				var linkedin = new Outlet();
				linkedin.Type = Outlet.outlet_type_linkedIn;
				SelectedCard.Outlets.Add(linkedin);

				var pinterest = new Outlet();
                pinterest.Type = Outlet.outlet_type_pinterest;
				SelectedCard.Outlets.Add(pinterest);

				cardFront.AddEmptySocialMedia(SelectedCard.Outlets.Count());
			}         }         public UIImageView CreateSnapshot(){
			var cardFront = ContainerView.Subviews.Where(c => c.GetType() == typeof(CardFront)).FirstOrDefault() as CardFront;
            if (cardFront != null)
            {                 var imgView = ViewUtils.ImageByRenderingView(cardFront);                 imgView.ContentMode = UIViewContentMode.ScaleToFill;                 return imgView;             }              return new UIImageView();         }         #endregion            #region static         public static nfloat GetCalculatedHeight()         {             nfloat result = 200;             nfloat ratio = result / 320;              var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;             if (appDelegate == null) return result;             if (appDelegate.Window == null) return result;              result = appDelegate.Window.Bounds.Width * ratio;             return result;         }         #endregion     } }  