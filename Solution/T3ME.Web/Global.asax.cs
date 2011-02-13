using MvcLibrary.Bootstrapper;
using System.Web.Compilation;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Web;
using System.Net;

namespace T3ME.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public MvcApplication()
        {
            this.BeginRequest += new EventHandler(MvcApplication_BeginRequest);
        }

        protected void Application_Start()
        {
            Bootstrapper.Run();
        }

        private void MvcApplication_BeginRequest(object sender, EventArgs e)
        {
            MvcApplication app = (MvcApplication)sender;

            string host = app.Request.ServerVariables["HTTP_HOST"];

            if ((host ?? "").StartsWith("www.", StringComparison.InvariantCultureIgnoreCase))
            {
                HttpResponse response = app.Response;

                Uri requestUrl = app.Request.Url;
                string redirectUrl = requestUrl.Scheme + "://" + host.Substring("www.".Length) + requestUrl.PathAndQuery;

                response.Clear();
                response.StatusCode = (int)HttpStatusCode.MovedPermanently;
                response.RedirectLocation = redirectUrl;
                response.End();
            }
        }
    }
}