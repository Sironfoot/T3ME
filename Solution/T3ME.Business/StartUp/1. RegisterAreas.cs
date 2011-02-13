using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcLibrary.Bootstrapper;
using System.Web.Mvc;

namespace T3ME.Business.StartUp
{
    [BootstrapperPriority(Priority = 1)]
    public class RegisterAreas : IBootstrapperTask
    {
        public void Execute()
        {
            AreaRegistration.RegisterAllAreas();
        }
    }
}