using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NHibernate;
using MvcLibrary.NHibernate.SessionManagement.Impl;

namespace MvcLibrary.NHibernate.SessionManagement
{
    public class SessionPerRequestModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.EndRequest += new EventHandler(Context_EndRequest);
        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            ISession session = context.Items[HttpSessionProvider.SessionKey] as ISession;
            if (session != null)
            {
                if (session.IsOpen)
                {
                    session.Close();
                }
                context.Items.Remove(HttpSessionProvider.SessionKey);
            }
        }

        public void Dispose() { }
    }
}