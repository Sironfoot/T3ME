using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MvcLibrary.Utils
{
    public static class DateTimeExtensions
    {
        public static string ToStringWithOrdinal(this DateTime value, string format)
        {
            format = format ?? "";

            string ordinal = value.Day.GetOrdinal();
            format = Regex.Replace(format, @"(\bdd\b|\bd\b)", "$1{0}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return value.ToString(format).Replace("{0}", ordinal);
        }

        /// <summary>
        /// Converts a regular DateTime to a RFC822 date string.
        /// </summary>
        /// <returns>The specified date formatted as a RFC822 date string.</returns>
        /// <remarks>http://www.extensionmethod.net/Details.aspx?ID=90</remarks>
        public static string ToRFC822DateString(this DateTime date)
        {
            int offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
            string timeZone = "+" + offset.ToString().PadLeft(2, '0');
            if (offset < 0)
            {
                int i = offset * -1;
                timeZone = "-" + i.ToString().PadLeft(2, '0');
            }
            return date.ToString("ddd, dd MMM yyyy HH:mm:ss " + timeZone.PadRight(5, '0'), System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        }

        public static string ToHtml5TimeString(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddThh:mm:ss") + "Z";
        }

        public static string ToTwitterString(this DateTime date)
        {
            return date.ToUniversalTime().ToTwitterString(DateTime.UtcNow);
        }

        public static string ToTwitterString(this DateTime date, DateTime compareTo)
        {
            TimeSpan timeSpan = new TimeSpan(compareTo.Ticks - date.Ticks);

            if (timeSpan.TotalMinutes < 1.0)
            {
                int seconds = (int)Math.Round(timeSpan.TotalSeconds, 0, MidpointRounding.AwayFromZero);

                if (seconds < 10)
                {
                    return "less than 10 seconds ago";
                }
                else if (seconds >= 10 && seconds < 20)
                {
                    return "less than 20 seconds ago";
                }
                else if (seconds >= 20 && seconds < 45)
                {
                    return "half a minute ago";
                }
                else
                {
                    return "less than a minute ago";
                }
            }
            else if (timeSpan.TotalMinutes >= 1.0 && timeSpan.TotalMinutes < 45.0)
            {
                int minutes = (int)Math.Round(timeSpan.TotalMinutes, 0, MidpointRounding.AwayFromZero);

                return minutes + " minute" + (minutes > 1 ? "s" : "") + " ago";
            }
            else if (timeSpan.TotalMinutes >= 45.0 && timeSpan.TotalHours <= 24.5)
            {
                int hours = (int)Math.Round(timeSpan.TotalHours, 0, MidpointRounding.AwayFromZero);

                return "about " + hours + " hour" + (hours > 1 ? "s" : "") + " ago";
            }
            else
            {
                return date.ToStringWithOrdinal("h:mm tt MMM d");
            }
        }
    }
}