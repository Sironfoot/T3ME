using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MvcLibrary.Utils
{
    public static class StringExtensions
    {
        /// <summary>
        ///     Converts null references to a blank string, trims off any whitespace
        ///     characters at the end of the string.
        /// </summary>
        /// <param name="input">String input to sanitise</param>
        public static string Sanitise(this string input)
        {
            return input == null ? "" : input.Trim();
        }

        /// <summary>
        ///     Capitalises the first letter of a string, while lowercasing the rest of the string.
        ///     Blank, empty or NULL strings will simply be returned.
        /// </summary>
        /// <param name="input">Input string to capitalise first letter</param>
        public static string CapitaliseFirstLetter(this string input)
        {
            if (String.IsNullOrWhiteSpace(input)) return input;

            char firstLetter = input.Trim()[0];
            return firstLetter.ToString().ToUpper() + input.Substring(1).ToLower();
        }

        /// <summary>
        ///     Encodes a string into a safe, SEO friendly format that can be safely used as URLs for links
        /// </summary>
        /// <param name="urlToEncode">The string to convert into a safe/friendly URL</param>
        public static string ToFriendlyUrl(this string urlToEncode)
        {
            urlToEncode = (urlToEncode ?? "").Trim().ToLower();

            StringBuilder url = new StringBuilder();

            foreach (char ch in urlToEncode)
            {
                switch (ch)
                {
                    case ' ':
                        url.Append('-');
                        break;
                    case '&':
                        url.Append("and");
                        break;
                    case '\'':
                        break;
                    default:
                        if ((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z'))
                        {
                            url.Append(ch);
                        }
                        else
                        {
                            url.Append('-');
                        }
                        break;
                }
            }

            return Regex.Replace(url.ToString(), @"\-+", "-").Trim('-');
        }
    }
}