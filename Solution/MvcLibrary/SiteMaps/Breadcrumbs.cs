using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace MvcLibrary.SiteMaps
{
    public class Breadcrumbs
    {
        protected readonly SiteMapNode RootNode = null;
        protected readonly HtmlTextWriter Writer = null;

        public Breadcrumbs(SiteMapNode rootNode, HtmlTextWriter writer)
        {
            this.RootNode = rootNode;
            this.Writer = writer;
        }

        public void Render()
        {
            Writer.RenderBeginTag(HtmlTextWriterTag.Ol);

            if (SiteMap.CurrentNode == RootNode)
            {
                Writer.AddAttribute(HtmlTextWriterAttribute.Class, "selected");
                Writer.RenderBeginTag(HtmlTextWriterTag.Li);

                Writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                Writer.Write(RootNode.Title);
                Writer.RenderEndTag();

                Writer.RenderEndTag();
            }
            else
            {
                IList<SiteMapNode> parents = ListAllParents(SiteMap.CurrentNode).Reverse().ToList();

                foreach (SiteMapNode node in parents)
                {
                    Writer.RenderBeginTag(HtmlTextWriterTag.Li);

                    Writer.AddAttribute(HtmlTextWriterAttribute.Href, node.Url);
                    Writer.RenderBeginTag(HtmlTextWriterTag.A);
                    Writer.Write(node.Title);
                    Writer.RenderEndTag();

                    Writer.RenderEndTag();
                }

                Writer.AddAttribute(HtmlTextWriterAttribute.Class, "selected");
                Writer.RenderBeginTag(HtmlTextWriterTag.Li);

                Writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                Writer.Write(SiteMap.CurrentNode.Title);
                Writer.RenderEndTag();

                Writer.RenderEndTag();
            }

            Writer.RenderEndTag();
        }

        protected IList<SiteMapNode> ListAllParents(SiteMapNode node)
        {
            List<SiteMapNode> nodes = new List<SiteMapNode>();

            SiteMapNode parentNode = node.ParentNode;

            if (parentNode != null)
            {
                nodes.Add(parentNode);
                nodes.AddRange(ListAllParents(parentNode));
            }

            return nodes;
        }
    }
}