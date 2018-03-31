using System;

using Foundation;
using UIKit;

namespace Slink.iOS
{
    public partial class NewCardEditableTableViewCell : BaseTableViewCell
    {
        public static readonly NSString Key = new NSString("NewCardEditableTableViewCell");
        public static readonly UINib Nib;
        public static readonly nfloat DefaultHeight = 44;

        protected NewCardEditableTableViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static NewCardEditableTableViewCell Create()
        {
            return UINib.FromName("NewCardEditableTableViewCell", NSBundle.MainBundle).Instantiate(null, null)[0] as NewCardEditableTableViewCell;
        }


        public override void Reset()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;

            TextField.Text = null;
            TextField.AttributedPlaceholder = null;
            TextField.SpellCheckingType = UITextSpellCheckingType.No;
            TextField.AutocorrectionType = UITextAutocorrectionType.No;
            TextField.AutocapitalizationType = UITextAutocapitalizationType.Words;
            TextField.ReturnKeyType = UIReturnKeyType.Done;
        }

        public void BindDataToView(NewCardModel model)
        {
            Reset();

            TextField.Text = model.Title;
            TextField.AttributedPlaceholder = new NSAttributedString(model.Placeholder, TextField.Font, UIColor.White);
            TextField.RemoveTarget(null, null, UIControlEvent.EditingChanged);
            TextField.EditingChanged += (sender, e) =>
            {
                if (model.Placeholder.Equals(NewCardShared.new_card_model_title_placeholder, StringComparison.InvariantCultureIgnoreCase))
                {
                    model.SelectedCard.UpdateStringProperty(() => model.SelectedCard.Title, TextField.Text.Trim());
                    model.SelectedCard.ShowFront();
                }
                else if (model.Placeholder.Equals(NewCardShared.new_card_model_display_name_placeholder, StringComparison.InvariantCultureIgnoreCase))
                {
                    model.SelectedCard.UpdateStringProperty(() => model.SelectedCard.UserDisplayName, TextField.Text.Trim());
                    model.SelectedCard.ShowFront();
                }

                else if (model.Placeholder.Equals(NewCardShared.new_card_model_company_name, StringComparison.InvariantCultureIgnoreCase))
                {
                    model.SelectedCard.UpdateStringProperty(() => model.SelectedCard.CompanyName, TextField.Text.Trim());
                    model.SelectedCard.ShowBack();
                }

                NSNotificationCenter.DefaultCenter.PostNotificationName(Strings.InternalNotifications.notification_table_row_editing_changed, null);

            };
            TextField.RemoveTarget(null, null, UIControlEvent.EditingDidEndOnExit);
            TextField.EditingDidEndOnExit += (sender, e) =>
            {
                TextField.ResignFirstResponder();
            };
        }
    }
}