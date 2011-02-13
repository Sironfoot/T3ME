using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.Mvc;
using System.Web;

namespace MvcLibrary.SiteMaps
{
    public static class SiteMapHtmlHelpers
    {
        public static SiteMapMenu SiteMapMenu(this HtmlHelper html)
        {
            HtmlTextWriter writer = new HtmlTextWriter(html.ViewContext.HttpContext.Response.Output);
            return new SiteMapMenu(SiteMap.RootNode, writer);
        }

        public static Breadcrumbs Breadcrumbs(this HtmlHelper html)
        {
            HtmlTextWriter writer = new HtmlTextWriter(html.ViewContext.HttpContext.Response.Output);
            return new Breadcrumbs(SiteMap.RootNode, writer);
        }
    }
}