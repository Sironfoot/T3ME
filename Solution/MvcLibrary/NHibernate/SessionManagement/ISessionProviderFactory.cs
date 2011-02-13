using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcLibrary.NHibernate.SessionManagement
{
    public interface ISessionProviderFactory
    {
        SessionProviderBase GetSessionProvider();
    }
}
