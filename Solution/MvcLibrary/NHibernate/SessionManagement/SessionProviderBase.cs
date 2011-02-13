using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using System.Web;

namespace MvcLibrary.NHibernate.SessionManagement
{
    public abstract class SessionProviderBase
    {
        protected static readonly ISessionFactory SessionFactory;

        static SessionProviderBase()
        {
            SessionFactory = new Configuration().Configure(HttpRuntime.AppDomainAppPath + "App_Data/NHibernate.cfg.xml").BuildSessionFactory();
        }

        public abstract ISession GetSession();
        public abstract void CloseSession();
    }
}