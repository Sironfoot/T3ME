using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace MvcLibrary.Ioc
{
    public static class IocHelper
    {
        private static IWindsorContainer _container = null;
        private static object _syncObject = new object();

        public static IWindsorContainer Container()
        {
            if (_container == null)
            {
                lock (_syncObject)
                {
                    if (_container == null)
                    {
                        _container = new WindsorContainer(new XmlInterpreter("App_Data/ioc.xml"));
                    }
                }
            }

            return _container;
        }

        public static void DisposeContainer()
        {
            if (_container != null)
            {
                lock (_syncObject)
                {
                    if (_container != null)
                    {
                        _container.Dispose();
                        _container = null;
                    }
                }
            }
        }
    }
}
