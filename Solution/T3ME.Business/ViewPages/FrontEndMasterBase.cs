using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Business.ViewModels;
using T3ME.Domain.Models;

namespace T3ME.Business.ViewPages
{
    public class FrontEndMasterBase : System.Web.Mvc.ViewMasterPage
    {
        private MasterFrontEndView _masterView = null;
        protected MasterFrontEndView MasterView
        {
            get
            {
                if (_masterView == null)
                {
                    _masterView = ViewData["MasterView"] as MasterFrontEndView;
                }

                return _masterView;
            }
        }

        protected Application App
        {
            get { return MasterView.App; }
        }

        protected Language DefaultLanguage
        {
            get { return MasterView.DefaultLanguage; }
        }
    }
}