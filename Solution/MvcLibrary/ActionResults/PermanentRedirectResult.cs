using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Net;

namespace MvcLibrary.ActionResults
{
    public class PermanentRedirectResult : RedirectResult
    {
        public PermanentRedirectResult(string url) : base(url) { }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;

            //response.Status = "301 Moved Permanently";
            response.StatusCode = (int)HttpStatusCode.MovedPermanently;
            response.RedirectLocation = base.Url;
            response.End();
        }
    }
}