using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;

namespace MvcLibrary.Utils.Monitoring
{
    public class PageTimerModule : IHttpModule
    {
        private const string KEY = "__MvcLibrary.Utils.Monitoring.PageTimerModule.KEY";

        public void Dispose() { }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += delegate(object sender, EventArgs e)
            {
                HttpContext httpContext = ((HttpApplication)sender).Context;

                Stopwatch timer = new Stopwatch();
                httpContext.Items.Add(KEY, timer);
                timer.Start();
            };

            context.PreSendRequestHeaders += delegate(object sender, EventArgs e)
            {
                HttpContext httpContext = ((HttpApplication)sender).Context;

                Stopwatch timer = httpContext.Items[KEY] as Stopwatch;

                if (timer != null)
                {
                    timer.Stop();

                    double seconds = (double)timer.ElapsedTicks / (double)Stopwatch.Frequency;
                    string result = String.Format("{0:F4} seconds ({1:F0} req/sec)", seconds, 1 / seconds);

                    httpContext.Response.AddHeader("X-RequestExecutionTime", result);

                    if (httpContext.Response.ContentType == "text/html")
                    {
                        //httpContext.Response.Write("<div style=\"position: fixed; right: 5px; bottom: 5px; font-size: 14px;\">Page Execution Time: " + result + "</div>");
                    }
                }
            };
        }
    }
}