using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Linq.Expressions;
using MvcLibrary.Utils;
using System.Xml.Linq;
using System.Web;

namespace MvcLibrary.ActionResults
{
    public class RssResult<T> : ActionResult
    {
        protected string _title = null;
        protected string _link = null;
        protected string _description = null;

        protected IEnumerable<T> _dataSource = null;

        public RssResult(string title, string link, string description)
        {
            this._title = title;
            this._link = link;
            this._description = description;
        }

        public IEnumerable<T> DataSource
        {
            set { _dataSource = value; }
            get { return _dataSource; }
        }

        protected Func<T, string> _titleField = null;
        protected Func<T, string> _linkField = null;
        protected Func<T, string> _descriptionField = null;
        protected Func<T, string> _authorField = null;
        protected Func<T, string> _commentsField = null;
        protected Func<T, string> _categoryField = null;
        protected Func<T, DateTime> _pubDateField = null;
        protected Func<T, string> _guidField = null;

        public void SetDataSourceFields(Func<T, string> title, Func<T, string> link,
            Func<T, string> description, Func<T, string> author, Func<T, DateTime> pubDate)
        {
            this._titleField = title;
            this._linkField = link;
            this._descriptionField = description;
            this._authorField = author;
            this._pubDateField = pubDate;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            IList<XElement> elements = new List<XElement>();

            foreach (T item in _dataSource)
            {
                XElement element = new XElement("item",
                    new XElement("title", _titleField(item)),
                    new XElement("link", _linkField(item)),
                    new XElement("description", _descriptionField(item)),
                    _authorField != null ? new XElement("author", _authorField(item)) : null,
                    _guidField != null ? new XElement("guid", _guidField(item)) : null,
                    _commentsField != null ? new XElement("comments", _commentsField(item)) : null,
                    _categoryField != null ? new XElement("category", _categoryField(item)) : null,
                     _pubDateField != null ? new XElement("pubDate", _pubDateField(item).ToRFC822DateString()) : null);

                elements.Add(element);
            }

            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("rss",
                    new XAttribute("version", "2.0"),
                    new XElement("channel",
                        new XElement("title", _title),
                        new XElement("link", _link),
                        new XElement("description", _description),
                        elements
                    )
                )
            );

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = "application/rss+xml";
            response.Write(doc.ToString(SaveOptions.None));
        }
    }
}