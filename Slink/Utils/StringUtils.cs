using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Slink
{
    public static class StringUtils
    {
        public static bool IsNumeric(string subject)
        {
            if (String.IsNullOrEmpty(subject))
                return false;


            foreach (char c in subject)
            {
                if (!Char.IsNumber(c))
                {
                    return false;
                }
            }
            return true;
        }
        public static string RemoveNonIntegers(string str)
        {
            return (!String.IsNullOrEmpty(str)) ? new Regex(@"[^\d]").Replace(str, "") : String.Empty;
        }
        public static string RemoveNewLineAndCarriageReturn(string str)
        {
            return (!String.IsNullOrEmpty(str)) ? Regex.Replace(str, @"\n|\r", "") : String.Empty;
        }
        public static string RemoveQuotes(string str)
        {
            return (!String.IsNullOrEmpty(str)) ? (str.Replace('"', ' ').Trim()).Replace("'", " ").Trim() : String.Empty;
        }
        public static string SplitCamelCaseToWords(string str)
        {
            return (!String.IsNullOrEmpty(str)) ? Regex.Replace(str, "(\\B[A-Z])", " $1") : String.Empty;
        }

        public static string Left(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return String.Empty;


            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength ? value : value.Substring(0, maxLength));
        }
        public static string FirstWord(string str)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;

            if (str.IndexOf(" ") != -1)
            {
                str = str.Substring(0, str.IndexOf(" "));
            }

            return str;
        }
        public static string GetPrettyDate(DateTime date)
        {
            if (date == null)
                return String.Empty;

            return DateUtils.GetRelativeDate(date);
        }
        public static string RemoveLeadingSymbol(string str, string symbol)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;

            if (str.Substring(0, 1) == symbol)
            {
                str = str.Substring(1);
            }

            return str;
        }

        public static bool IsValidURL(string str)
        {
            if (String.IsNullOrEmpty(str)) return false;

            return Uri.IsWellFormedUriString(str, UriKind.Absolute);
        }
        public static bool IsValidEmail(string str)
        {
            if (String.IsNullOrEmpty(str)) return false;

            return Regex.Match(str, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success;
        }
        public static bool IsValidUsername(string str)
        {
            if (str.Length <= 5)
            {
                return false;
            }
            if (str.Length >= 20)
            {
                return false;
            }

            string allowableLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPRSTUVWXYZ0123456789_";

            foreach (char c in str)
            {
                if (!allowableLetters.Contains(c.ToString()))
                    return false;
            }


            return true;
        }
        public static string ToHexString(byte[] bytes)
        {
            var result = String.Concat(bytes.Select(b => b.ToString("X2")).ToArray());
            return result;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes, 0, base64EncodedBytes.Length);
        }
        public static string TrimUnwantedCharactersFromUsername(string str)
        {
            return Regex.Replace(str, "[^A-Za-z0-9_]", "");
        }
        public static List<PasswordRules> IsValidPassword(string str)
        {
            List<PasswordRules> returnList = new List<PasswordRules>();

            if (str.Length <= 5)
            {
                returnList.Add(PasswordRules.OverFiveCharacters);
            }
            if (!Regex.IsMatch(str, "[A-Z]"))
            {
                returnList.Add(PasswordRules.AtLeastOneCapitol);
            }
            if (!Regex.IsMatch(str, "[a-z]"))
            {
                returnList.Add(PasswordRules.AtLeastOneLowercase);
            }
            if (!Regex.IsMatch(str, "[0-9]"))
            {
                returnList.Add(PasswordRules.AtLeastOneNumber);
            }
            if (str.Length > 42)
            {
                returnList.Add(PasswordRules.UnderFourtyTwoCharacters);
            }

            return (returnList.Count == 0) ? null : returnList;
        }
        public enum PasswordRules
        {
            OverFiveCharacters = 1,
            AtLeastOneCapitol = 2,
            AtLeastOneLowercase = 3,
            AtLeastOneNumber = 4,
            UnderFourtyTwoCharacters = 5
        }
    }
}

