using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;
using Castle.Core;
using System.Web.Routing;

namespace MvcLibrary.Ioc
{
    public class IocControllerFactory : DefaultControllerFactory
    {
        public IocControllerFactory()
        {
            Type controllerInterfaceType = typeof(IController);

            List<Type> controllerTypes = new List<Type>();

            foreach (Assembly assembly in AssemblyLocator.GetBinFolderAssemblies())
            {
                controllerTypes.AddRange(from t in assembly.GetTypes()
                                         where controllerInterfaceType.IsAssignableFrom(t) &&
                                            t.IsInterface == false &&
                                            t.IsAbstract == false
                                         select t);
            }

            foreach (Type type in controllerTypes)
            {
                IocHelper.Container().AddComponentLifeStyle(type.FullName, type, LifestyleType.Transient);
            }
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType != null)
            {
                return (IController)IocHelper.Container().Resolve(controllerType);
            }

            return base.GetControllerInstance(requestContext, controllerType);
        }
    }
}
