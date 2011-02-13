using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MvcLibrary.Utils;
using T3ME.Domain.Models;

namespace T3ME.Business.Utils
{
    public static class T3meUtils
    {
        private const string URL_REGEX = @"(((http|https)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";
        private const string TWEETER_USERNAME_REGEX = @"(@([a-z0-9\-_]+))";
        private const string HASH_TAG_REGEX = @"#(([a-z_]+[0-9_]*[a-z0-9_]+)|([0-9_]+[a-z_]+[a-z0-9_]+))";

        public static string ParseTweetText(this HtmlHelper html, string message)
        {
            message = HttpUtility.HtmlEncode(message);

            message = Regex.Replace(message, URL_REGEX, "<a href=\"$1\">$1</a>", RegexOptions.IgnoreCase);
            message = Regex.Replace(message, TWEETER_USERNAME_REGEX, "<a href=\"http://twitter.com/$2\">$1</a>", RegexOptions.IgnoreCase);

            if (message.IndexOf('#') != -1)
            {
                RouteBuilder routeBuilder = html.CurrentRoute();
                routeBuilder.RemoveRouteValue("page");
                routeBuilder.RemoveRouteValue("format");

                if (routeBuilder.Action.Equals("status", StringComparison.InvariantCultureIgnoreCase))
                {
                    routeBuilder.Action = "Latest";
                    routeBuilder.RemoveRouteValue("twitterId");
                    routeBuilder.RemoveRouteValue("message");
                }
                else
                {
                    // TODO: Temporary fix to deal with Ajax Requests where the HTTP
                    // request isn't been dealt by the 'Display' Controller
                    if (routeBuilder.Controller != "Display")
                    {
                        routeBuilder.Controller = "Display";
                        routeBuilder.Action = "Latest";

                        routeBuilder.RemoveRouteValue("lastTweetId");
                        routeBuilder.RemoveRouteValue("order");
                    }
                }
                

                message = Regex.Replace(message, HASH_TAG_REGEX, m =>
                {
                    string hashTag = m.Groups[0].Value;
                    routeBuilder.AddRouteValue("search", hashTag);

                    return "<a href=\"" + routeBuilder + "\">" + hashTag + "</a>";
                },
                RegexOptions.IgnoreCase);
            }

            return message;
        }

        public static string ParseTweetForPageTitle(this string message)
        {
            message = (message ?? "");

            message = HttpUtility.HtmlEncode(message);

            message = Regex.Replace(message, HASH_TAG_REGEX, "$1", RegexOptions.IgnoreCase);
            message = Regex.Replace(message, URL_REGEX, "", RegexOptions.IgnoreCase);

            return message.Trim();
        }

        public static IList<string> GetHashTags(this string message)
        {
            IList<string> tags = new List<string>();

            MatchCollection matches = Regex.Matches(message ?? "", HASH_TAG_REGEX, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                string tag = match.Groups[1].Value;

                if (!tags.Any(t => t.Equals(tag, StringComparison.InvariantCultureIgnoreCase)))
                {
                    tags.Add(tag);
                }
            }

            return tags;
        }

        private static readonly Regex RETWEET_REGEX = new Regex(@"RT\:?\ \@", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsRetweet(this string statusText)
        {
            if (String.IsNullOrWhiteSpace(statusText)) return false;

            return RETWEET_REGEX.IsMatch(statusText);
        }
    }
}