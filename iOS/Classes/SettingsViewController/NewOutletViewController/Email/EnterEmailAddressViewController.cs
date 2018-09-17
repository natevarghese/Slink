using System;

using UIKit;

namespace Slink.iOS
{
    public partial class EnterEmailAddressViewController : BaseLandingViewController
    {
        public EnterEmailAddressViewController() : base("EnterEmailAddressViewController") { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RemoveBackBarButtonTitle();

            EmailAddressTextField.KeyboardType = UIKeyboardType.EmailAddress;
            EmailAddressTextField.Placeholder = Strings.Basic.email_address;
            EmailAddressTextField.AutocorrectionType = UITextAutocorrectionType.No;
            EmailAddressTextField.BecomeFirstResponder();
            EmailAddressTextField.EditingDidEndOnExit += (sender, e) =>
            {
                EmailAddressTextField.ResignFirstResponder();
            };

            SetProgressButton(ValidateButton);
        }

        protected override bool ValidateAllFields()
        {
            string emailAddress = EmailAddressTextField.Text.Trim();

            bool allFieldsValid = true;

            if (String.IsNullOrEmpty(emailAddress) || !StringUtils.IsValidEmail(emailAddress))
            {
                EmailAddressTextField.SetInvalid();
                allFieldsValid = false;
            }

            return allFieldsValid;
        }
        public override TPKeyboardAvoiding.TPKeyboardAvoidingScrollView GetScrollView()
        {
            return ScrollView;
        }
        async partial void ValidateButtonClicked(Foundation.NSObject sender)
        {
            if (!IsConnected) return;

            View.EndEditing(true);

            string emailAddress = EmailAddressTextField.Text.Trim();

            bool allFieldsValid = ValidateAllFields();
            if (!allFieldsValid) return;

            ShowHud(Strings.Hud.please_wait);

            try
            {
                var result = await WebServices.EmailController.SendValidationCode(emailAddress);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Transporter.SharedInstance.SetObject("email", emailAddress);

                    var vc = new EnterEmailAddressVerificationCodeViewController();
                    NavigationController.PushViewController(vc, true);
                }
                else
                {
                    EmailAddressTextField.SetInvalid();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            HideHud();
        }
    }
}

