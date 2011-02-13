using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using T3ME.Business.Controllers;
using T3ME.Domain.Repositories;

namespace T3ME.Web.Controllers
{
    public class AboutController : MasterController
    {
        public AboutController(IApplicationRepository appRepository, ITweetRepository tweetRepository,
            ITweeterRepository tweeterRepository, ILanguageRepository languageRepository)
            : base(appRepository, tweetRepository, tweeterRepository, languageRepository) { }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Legal()
        {
            return View();
        }
    }
}