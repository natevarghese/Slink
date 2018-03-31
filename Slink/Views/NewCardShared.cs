using System;
using System.Collections.Generic;
using Plugin.DeviceInfo;
using Plugin.DeviceInfo.Abstractions;
namespace Slink
{
    public class NewCardShared
    {
        public Card SelectedCard;
        List<NewCardModel> TableItems;

        public static string AddNewOutlet = "Add Outlet +";

        public static string new_card_model_border_color = "Border Color";
        public static string new_card_model_background_color = "Background Color";
        public static string new_card_model_company_name = "Company Name";
        public static string new_card_model_company_name_text_color = "Company Name Text Color";
        public static string new_card_model_name_placeholder = "Enter Card Name";
        public static string new_card_model_display_name_placeholder = "Your Name";
        public static string new_card_model_title_placeholder = "Your Title";

        public List<NewCardModel> GetTableItems()
        {
            //if (TableItems != null) return TableItems;

            TableItems = new List<NewCardModel>();

            //header
            if (CrossDeviceInfo.Current.Platform == Platform.Android)
            {
                var header = new NewCardModel();
                header.Editable = true;
                header.SelectedCard = SelectedCard;
                header.IsHeader = true;
                TableItems.Add(header);
            }

            var cardUserDisplayNameModel = new NewCardModel();
            cardUserDisplayNameModel.Editable = true;
            cardUserDisplayNameModel.Title = (SelectedCard != null) ? SelectedCard.UserDisplayName : null;
            cardUserDisplayNameModel.Placeholder = new_card_model_display_name_placeholder;
            cardUserDisplayNameModel.SelectedCard = SelectedCard;
            TableItems.Add(cardUserDisplayNameModel);

            var cardTitleModel = new NewCardModel();
            cardTitleModel.Editable = true;
            cardTitleModel.Title = (SelectedCard != null) ? SelectedCard.Title : null;
            cardTitleModel.Placeholder = new_card_model_title_placeholder;
            cardTitleModel.SelectedCard = SelectedCard;
            TableItems.Add(cardTitleModel);

            var borderColorModel = new NewCardModel();
            borderColorModel.Title = new_card_model_border_color;
            borderColorModel.ColorHexString = SelectedCard.BorderColor;
            TableItems.Add(borderColorModel);

            var backgroundColorModel = new NewCardModel();
            backgroundColorModel.Title = new_card_model_background_color;
            backgroundColorModel.ColorHexString = SelectedCard.BackgroundColor;
            TableItems.Add(backgroundColorModel);

            var cardCompanyName = new NewCardModel();
            cardCompanyName.Editable = true;
            cardCompanyName.Title = (SelectedCard != null) ? SelectedCard.CompanyName : null;
            cardCompanyName.Placeholder = new_card_model_company_name;
            cardCompanyName.SelectedCard = SelectedCard;
            TableItems.Add(cardCompanyName);

            var companyNameTextColorModel = new NewCardModel();
            companyNameTextColorModel.Title = new_card_model_company_name_text_color;
            companyNameTextColorModel.ColorHexString = SelectedCard.CompanyNameTextColor;
            TableItems.Add(companyNameTextColorModel);

            var outletTitleModel = new NewCardModel();
            outletTitleModel.IsTitle = true;
            outletTitleModel.Title = "";
            TableItems.Add(outletTitleModel);

            foreach (Outlet outlet in SelectedCard.Outlets)
            {
                var outletModel = new NewCardModel();
                outletModel.Outlet = outlet;
                TableItems.Add(outletModel);
            }

            //footer
            if (CrossDeviceInfo.Current.Platform == Platform.Android)
            {
                TableItems.Add(null);
            }

            return TableItems;
        }
    }

    public class NewCardModel
    {

        public Outlet Outlet;
        public bool IsTitle;
        public bool IsHeader;
        public string Title;
        public string ColorHexString;

        public bool Editable;
        public string Placeholder;
        public Card SelectedCard;
    }
}
