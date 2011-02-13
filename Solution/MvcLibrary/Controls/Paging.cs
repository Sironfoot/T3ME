using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using MvcLibrary.Utils;
using System.Web.Routing;

namespace MvcLibrary.Controls
{
    public static class PagingExtensions
    {
        public static Paging PagingLinks(this HtmlHelper html, int recordsPerPage, int totalRecords)
        {
            HtmlTextWriter writer = new HtmlTextWriter(html.ViewContext.HttpContext.Response.Output);

            return new Paging(html, writer, recordsPerPage, totalRecords);
        }
    }

    public class Paging
    {
        protected readonly HtmlHelper HtmlHelper = null;
        protected readonly HtmlTextWriter Writer = null;

        protected int RecordsPerPage = 1;
        protected int TotalRecords = 0;
        protected int CurrentPage = 1;
        protected int CutOffRange = Int32.MaxValue;
        protected string PageRouteKey = "page";

        internal Paging(HtmlHelper htmlHelper, HtmlTextWriter writer, int recordsPerPage, int totalRecords)
        {
            this.HtmlHelper = htmlHelper;
            this.Writer = writer;
            this.RecordsPerPage = recordsPerPage.AtLeast(1);
            this.TotalRecords = totalRecords.AtLeast(0);
        }

        public Paging SetCurrentPage(int currentPage)
        {
            this.CurrentPage = currentPage.AtLeast(1);
            return this;
        }

        public Paging SetCutOffRange(int cutOffRange)
        {
            this.CutOffRange = cutOffRange.AtLeast(2);
            return this;
        }

        public Paging SetPageRouteKey(string routeKey)
        {
            PageRouteKey = routeKey;
            return this;
        }

        public void Render()
        {
            RouteBuilder routeBuilder = HtmlHelper.CurrentRoute();

            // <ul>
            Writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            int totalPages = ((int)Math.Ceiling((double)TotalRecords / (double)RecordsPerPage)).AtLeast(1);

            int sidePages = (int)Math.Ceiling((double)CutOffRange / 2.0);

            int startRecord = (CurrentPage - sidePages).AtLeast(1);
            int endRecord = (CurrentPage + sidePages).AtMost(totalPages);

            for (int page = startRecord; page <= endRecord; page++)
            {
                bool showStartEllipsis = page == startRecord && page > 1;
                bool showEndEllipsis = page == endRecord && page < totalPages;

                if (page == CurrentPage)
                {
                    Writer.AddAttribute(HtmlTextWriterAttribute.Class, "selected");
                }
                Writer.RenderBeginTag(HtmlTextWriterTag.Li);

                if (page == CurrentPage)
                {
                    Writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    Writer.Write(page);
                    Writer.RenderEndTag();
                }
                else
                {
                    routeBuilder.AddRouteValue(PageRouteKey, page);
                    string url = routeBuilder.ToString();

                    Writer.AddAttribute(HtmlTextWriterAttribute.Href, url);
                    Writer.RenderBeginTag(HtmlTextWriterTag.A);
                    Writer.Write((showStartEllipsis ? "..." : "") + page + (showEndEllipsis ? "..." : ""));
                    Writer.RenderEndTag();
                }

                Writer.RenderEndTag();
            }

            // </ul>
            Writer.RenderEndTag();
        }
    }
}