using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Configuration;
using System.Web;
using NHibernate;

namespace MvcLibrary.NHibernate.SessionManagement.Impl
{
    public class HttpSessionProvider : SessionProviderBase
    {
        internal static readonly string SessionKey = "__" + typeof(HttpSessionProvider).FullName + ".SessionKey";

        static HttpSessionProvider()
        {
            // Check that the SessionPerRequest HTTP Module has been registered so that we don't get memory leaks
            Type sessionPerRequestModule = typeof(SessionPerRequestModule);

            bool foundModule = false;

            HttpModulesSection modulesSection = WebConfigurationManager.GetSection("system.web/httpModules") as HttpModulesSection;
            if (modulesSection != null)
            {
                foreach (HttpModuleAction module in modulesSection.Modules)
                {
                    Type moduleType = Type.GetType(module.Type);

                    if (moduleType == sessionPerRequestModule)
                    {
                        foundModule = true;
                        break;
                    }
                }
            }

            if (!foundModule)
            {
                string message = "The '" + sessionPerRequestModule.FullName + "' HttpModule needs to be registered " +
                                 "in order to use the HttpSessionProvider implementation, please add it to your web.config's " +
                                 "<httpModules> section now.";

                throw new ConfigurationErrorsException(message);
            }
        }

        public override ISession GetSession()
        {
            HttpContext context = HttpContext.Current;

            ISession session = context.Items[SessionKey] as ISession;

            if (session == null)
            {
                session = SessionFactory.OpenSession();
                context.Items.Add(SessionKey, session);
            }

            if (!session.IsOpen)
            {
                session.Reconnect();
            }

            return session;
        }

        public override void CloseSession()
        {
            HttpContext context = HttpContext.Current;

            ISession session = context.Items[SessionKey] as ISession;

            if (session != null)
            {
                session.Close();
                context.Items.Remove(SessionKey);
            }
        }
    }
}