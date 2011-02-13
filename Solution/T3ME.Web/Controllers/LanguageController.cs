using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using T3ME.Business.Controllers;
using T3ME.Domain.Repositories;
using T3ME.Domain.Models;
using T3ME.Business.ViewModels.Forms;

namespace T3ME.Web.Controllers
{
    public class LanguageController : MasterController
    {
        public LanguageController(IApplicationRepository appRepository, ITweetRepository tweetRepository,
            ITweeterRepository tweeterRepository, ILanguageRepository languageRepository)
            : base(appRepository, tweetRepository, tweeterRepository, languageRepository) { }

        public ActionResult SetLanguage(LanguageSelectForm languageForm)
        {
            DefaultLanguage = LanguageRepository.FromId(languageForm.SelectedLanguage.GetValueOrDefault(-1));

            return Redirect(ReturnUrl);
        }

        public ActionResult ClearLanguage()
        {
            DefaultLanguage = null;

            return Redirect(ReturnUrl);
        }
    }
}