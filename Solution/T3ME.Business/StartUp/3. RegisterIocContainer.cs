using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcLibrary.Bootstrapper;
using System.Web.Mvc;
using MvcLibrary.Ioc;

namespace T3ME.Business.StartUp
{
    [BootstrapperPriority(Priority = 3)]
    public class RegisterIocContainer : IBootstrapperTask
    {
        public void Execute()
        {
            ControllerBuilder.Current.SetControllerFactory(new IocControllerFactory());
        }
    }
}