using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using MvcLibrary.Utils;

namespace MvcLibrary.SiteMaps
{
    public class SiteMapMenu
    {
        private SiteMapNode RootNode;
        private HtmlTextWriter Writer;

        private bool IsRootFlattened = false;
        private string CssClass = null;
        private string SelectedNodeCssClass = "selected";
        private string Id = null;
        private int MaxDepth = Int32.MaxValue;

        public SiteMapMenu(SiteMapNode rootNode, HtmlTextWriter writer)
        {
            this.RootNode = rootNode;
            this.Writer = writer;
        }

        public SiteMapMenu FlattenRoot()
        {
            this.IsRootFlattened = true;
            return this;
        }

        public SiteMapMenu SetCssClass(string cssClass)
        {
            this.CssClass = cssClass;
            return this;
        }

        public SiteMapMenu SetSelectedNodeCssClass(string selectedCssClass)
        {
            this.SelectedNodeCssClass = selectedCssClass;
            return this;
        }

        public SiteMapMenu SetId(string id)
        {
            this.Id = id;
            return this;
        }

        public SiteMapMenu SetMaxDepth(int depth)
        {
            this.MaxDepth = depth;
            return this;
        }

        public void Render()
        {
            if (!String.IsNullOrWhiteSpace(Id))
            {
                Writer.AddAttribute(HtmlTextWriterAttribute.Id, Id.Trim());
            }

            if (!String.IsNullOrWhiteSpace(CssClass))
            {
                Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass.Trim());
            }

            Writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            int currentDepth = 1;

            if (IsRootFlattened)
            {
                if (SiteMap.CurrentNode == RootNode)
                {
                    if (!String.IsNullOrWhiteSpace(SelectedNodeCssClass))
                    {
                        Writer.AddAttribute(HtmlTextWriterAttribute.Class, SelectedNodeCssClass);
                    }
                    Writer.RenderBeginTag(HtmlTextWriterTag.Li);
                    Writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    Writer.Write(RootNode.Title);
                    Writer.RenderEndTag();
                    Writer.RenderEndTag();
                }
                else
                {
                    Writer.RenderBeginTag(HtmlTextWriterTag.Li);
                    Writer.AddAttribute(HtmlTextWriterAttribute.Href, RootNode.Url);
                    Writer.RenderBeginTag(HtmlTextWriterTag.A);
                    Writer.Write(RootNode.Title);
                    Writer.RenderEndTag();
                    Writer.RenderEndTag();
                }

                foreach (SiteMapNode childNode in RootNode.ChildNodes)
                {
                    RenderRecursive(childNode, currentDepth);
                }
            }
            else
            {
                RenderRecursive(RootNode, currentDepth);
            }

            Writer.RenderEndTag();
        }

        private void RenderRecursive(SiteMapNode node, int currentDepth)
        {
            if (currentDepth > MaxDepth) return;

            if (SiteMap.CurrentNode == node)
            {
                if (!String.IsNullOrWhiteSpace(SelectedNodeCssClass))
                {
                    Writer.AddAttribute(HtmlTextWriterAttribute.Class, SelectedNodeCssClass);
                }
                Writer.RenderBeginTag(HtmlTextWriterTag.Li);
                Writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                Writer.Write(node.Title);
                Writer.RenderEndTag();
            }
            else
            {
                Writer.RenderBeginTag(HtmlTextWriterTag.Li);
                Writer.AddAttribute(HtmlTextWriterAttribute.Href, node.Url);
                Writer.RenderBeginTag(HtmlTextWriterTag.A);
                Writer.Write(node.Title);
                Writer.RenderEndTag();
            }

            if (node.ChildNodes.Count > 0)
            {
                Writer.RenderBeginTag(HtmlTextWriterTag.Ul);
                foreach (SiteMapNode child in node.ChildNodes)
                {
                    RenderRecursive(child, ++currentDepth);
                }
                Writer.RenderEndTag();
            }

            Writer.RenderEndTag();
        }
    }
}