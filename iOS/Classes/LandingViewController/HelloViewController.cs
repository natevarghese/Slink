using System;

using Foundation;
using Realms;
using Realms.Sync;
using UIKit;

namespace Slink.iOS
{
    public partial class HelloViewController : BaseLandingViewController
    {
        HelloShared Shared = new HelloShared();

        public HelloViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetProgressButton(StartButton);

            FirstNameTextField.KeyboardType = UIKeyboardType.EmailAddress;
            FirstNameTextField.Placeholder = "First Name";
            FirstNameTextField.AutocorrectionType = UITextAutocorrectionType.No;

            LastNameTextField.KeyboardType = UIKeyboardType.EmailAddress;
            LastNameTextField.Placeholder = "Last Name";
            LastNameTextField.AutocorrectionType = UITextAutocorrectionType.No;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var me = RealmUserServices.GetMe(true);

            FirstNameTextField.Text = me.FirstName;
            LastNameTextField.Text = me.LastName;
        }
        public override TPKeyboardAvoiding.TPKeyboardAvoidingScrollView GetScrollView()
        {
            return ScrollView;
        }
        protected override bool ValidateAllFields()
        {
            string firstName = FirstNameTextField.Text.Trim();
            string lastName = LastNameTextField.Text.Trim();

            bool allFieldsValid = true;

            //firstname validation
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

        partial void StartButtonClicked(Foundation.NSObject sender)
        {
            string firstName = FirstNameTextField.Text.Trim();
            string lastName = LastNameTextField.Text.Trim();

            bool allFieldsValid = ValidateAllFields();
            if (!allFieldsValid) return;

            var realm = RealmManager.SharedInstance.GetRealm(null);
            var me = RealmUserServices.GetMe(false);


            realm.Write(() =>
            {
                me.FirstName = firstName;
                me.LastName = lastName;
            });

            SlinkUser.SetNextHandelByNameIfNecessary();

            UIView.Animate(1, delegate
                {
                    View.Alpha = 0;
                }, delegate
                {
                    ApplicationExtensions.ShowOnboarding(true);
                });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Shared = null;
            }
        }
    }
}
