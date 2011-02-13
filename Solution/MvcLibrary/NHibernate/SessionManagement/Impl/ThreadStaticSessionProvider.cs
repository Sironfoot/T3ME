using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace MvcLibrary.NHibernate.SessionManagement.Impl
{
    public class ThreadStaticSessionProvider : SessionProviderBase
    {
        [ThreadStatic]
        private static ISession Session = null;

        public override ISession GetSession()
        {
            if (Session == null)
            {
                Session = SessionFactory.OpenSession();
            }

            if (!Session.IsOpen)
            {
                Session.Reconnect();
            }

            return Session;
        }

        public override void CloseSession()
        {
            if (Session != null)
            {
                Session.Close();
                Session = null;
            }
        }
    }
}