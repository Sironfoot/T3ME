using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Web.Hosting;
using System.Web.Routing;
using System.Collections.Specialized;

namespace MvcLibrary.SiteMaps
{
    public class RoutingSiteMapProvider : StaticSiteMapProvider
    {
        private SiteMapNode _rootNode = null;

        public override void Initialize(string name, NameValueCollection attributes)
        {
            base.Initialize(name, attributes);

            var siteMapFile = attributes["siteMapFile"] ?? "~/Web.sitemap";

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(HostingEnvironment.MapPath(siteMapFile));
            var rootSiteMapNode = xmlDoc.DocumentElement["siteMapNode"];

            var httpContext = new HttpContextWrapper(HttpContext.Current);
            var requestContext = new RequestContext(httpContext, new RouteData());
            _rootNode = AddNodeRecursive(rootSiteMapNode, null, requestContext);
        }

        private static string[] reservedNames = new[] { "title", "description", "roles" };

        private SiteMapNode AddNodeRecursive(XmlNode xmlNode, SiteMapNode parent, RequestContext context)
        {
            var routeValues = (from XmlNode attrib in xmlNode.Attributes
                               where !reservedNames.Contains(attrib.Name.ToLower())
                               select new { attrib.Name, attrib.Value }).ToDictionary(x => x.Name, x => (object)x.Value);

            RouteValueDictionary routeDict = new RouteValueDictionary(routeValues);
            VirtualPathData virtualPathData = RouteTable.Routes.GetVirtualPath(context, routeDict);

            if (virtualPathData == null)
            {
                string message = "RoutingSiteMapProvider is unable to locate Route for " +
                                 "Controller: '" + routeDict["controller"] + "', Action: '" + routeDict["action"] + "'. " +
                                 "Make sure a route has been defined for this SiteMap Node.";
                throw new InvalidOperationException(message);
            }

            string url = virtualPathData.VirtualPath;

            string title = xmlNode.Attributes["title"].Value;
            SiteMapNode node = new SiteMapNode(this, Guid.NewGuid().ToString(), url, title);

            base.AddNode(node, parent);

            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                AddNodeRecursive(childNode, node, context);
            }

            return node;
        }

        public override SiteMapNode BuildSiteMap()
        {
            return _rootNode;
        }

        protected override SiteMapNode GetRootNodeCore()
        {
            return _rootNode;
        }

        public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
        {
            if (node == _rootNode) return true;

            return base.IsAccessibleToUser(context, node);
        }
    }
}