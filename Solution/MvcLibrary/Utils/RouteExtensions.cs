using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace MvcLibrary.Utils
{
    public static class RouteExtensions
    {
        public static void MapRouteWithFormat(this RouteCollection routes, string name, string url, object defaults)
        {
            Match formatMatch = Regex.Match(url, @"\.{(.+?)}");

            if (formatMatch.Success)
            {
                string formatRouteValue = formatMatch.Groups[1].Value;

                RouteValueDictionary routeValues = new RouteValueDictionary(defaults);

                if (routeValues.ContainsKey(formatRouteValue))
                {
                    string urlSansFormat = Regex.Replace(url, @"\.{(.+?)}", "", RegexOptions.IgnoreCase);
                    routes.MapRoute(name + "DefaultFormat", urlSansFormat, defaults);
                }
            }

            routes.MapRoute(name, url, defaults);
        }
    }
}