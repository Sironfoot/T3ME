using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;

namespace MvcLibrary.Utils
{
    public class RouteBuilder
    {
        protected HtmlHelper Html = null;

        protected RouteValueDictionary RouteValues = null;

        public RouteBuilder(HtmlHelper html)
        {
            this.Html = html;

            RouteValues = new RouteValueDictionary();

            // Copy current Route's RouteData
            foreach (KeyValuePair<string, object> routeValue in Html.ViewContext.RouteData.Values)
            {
                RouteValues.Add(routeValue.Key, routeValue.Value);
            }

            // Copy QueryString values
            HttpRequestBase request = Html.ViewContext.HttpContext.Request;
            foreach (string key in request.QueryString.AllKeys)
            {
                if (!RouteValues.ContainsKey(key))
                {
                    RouteValues.Add(key, request.QueryString[key]);
                }
            }
        }

        public RouteBuilder AddRouteValue(string key, object value)
        {
            if (RouteValues.ContainsKey(key))
            {
                this.RouteValues[key] = value;
            }
            else
            {
                this.RouteValues.Add(key, value);
            }

            return this;
        }

        public RouteBuilder RemoveRouteValue(string key)
        {
            this.RouteValues.Remove(key);
            return this;
        }

        public bool ContainsKey(string key)
        {
            return this.RouteValues.ContainsKey(key);
        }

        public object this[string key]
        {
            get { return this.RouteValues[key]; }
        }

        public string Controller
        {
            get { return this.RouteValues["controller"].ToString(); }
            set { this.RouteValues["controller"] = value; }
        }

        public string Action
        {
            get { return this.RouteValues["action"].ToString(); }
            set { this.RouteValues["action"] = value; }
        }

        public override string ToString()
        {
            string controller = RouteValues["controller"].ToString();
            string action = RouteValues["action"].ToString();

            UrlHelper urlHelper = new UrlHelper(Html.ViewContext.RequestContext, RouteTable.Routes);
            return urlHelper.Action(action, controller, RouteValues);
        }
    }

    public static class HtmlExtensions
    {
        public static RouteBuilder CurrentRoute(this HtmlHelper html)
        {
            return new RouteBuilder(html);
        }
    }
}
