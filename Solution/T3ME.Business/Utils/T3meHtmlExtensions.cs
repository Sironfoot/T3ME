using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using MvcLibrary.Utils;
using T3ME.Domain.Models;

namespace T3ME.Business.Utils
{
    public static class T3meHtmlExtensions
    {
        public static string RssFeedLink(this HtmlHelper html)
        {
            RouteBuilder routeBuilder = html.CurrentRoute();

            if (routeBuilder.ContainsKey("format"))
            {
                routeBuilder.AddRouteValue("format", "rss");
                routeBuilder.RemoveRouteValue("page");

                return routeBuilder.ToString();
            }

            return "/Latest.rss";
        }

        public static string RssFeedTitle(this HtmlHelper html)
        {
            RouteBuilder routeBuilder = html.CurrentRoute();

            if (routeBuilder.ContainsKey("format"))
            {
                string title = SiteMap.CurrentNode.Title;

                string searchTerm = routeBuilder["search"] as string;
                if (!String.IsNullOrWhiteSpace(searchTerm))
                {
                    title += " containing \"" + searchTerm.Trim() + "\"";
                }

                return title;
            }

            return "Latest";
        }

        public static string RealUrl(this HtmlHelper html)
        {
            HttpRequestBase request = html.ViewContext.HttpContext.Request;
            return request.Url.Scheme + "://" + request.ServerVariables["HTTP_HOST"] + request.Url.PathAndQuery;
        }

        public static string NoRecordsMessage(this HtmlHelper html, Application app, bool isLanguageSet)
        {
            if (app.Tweets.Count > 0)
            {
                RouteBuilder routeBuilder = html.CurrentRoute();

                bool hasSearch = routeBuilder.ContainsKey("search");
                bool isLatestPage = routeBuilder.Action.Equals("latest", StringComparison.InvariantCultureIgnoreCase);

                if (!isLatestPage && !hasSearch && !isLanguageSet)
                {
                    return "No one has voted yet.";
                }
                else
                {
                    string message = "No tweets found. Try broadening your search criteria";

                    if (isLanguageSet)
                    {
                        message += ", or selecting a different language";
                    }

                    message += ".";

                    return message;
                }
            }
            else
            {
                return "No tweets have been indexed yet. Please wait a while, I'm sure there will be some soon.";
            }
        }
    }
}