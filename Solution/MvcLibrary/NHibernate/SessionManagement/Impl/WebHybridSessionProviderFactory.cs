using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MvcLibrary.NHibernate.SessionManagement.Impl
{
    public class WebHybridSessionProviderFactory : ISessionProviderFactory
    {
        public SessionProviderBase GetSessionProvider()
        {
            if (HttpContext.Current != null)
            {
                return new HttpSessionProvider();
            }
            else
            {
                return new ThreadStaticSessionProvider();
            }
        }
    }
}