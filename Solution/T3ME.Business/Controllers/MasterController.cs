using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using T3ME.Domain.Repositories;
using T3ME.Domain.Models;
using Twitterizer;
using System.Text.RegularExpressions;
using T3ME.Domain.ReportModels;
using T3ME.Business.ViewModels;
using System.Web;
using MvcLibrary.Utils;
using T3ME.Business.ViewModels.Forms;

namespace T3ME.Business.Controllers
{
    public abstract class MasterController : Controller
    {
        private const string REFERRER_URL_KEY = "__T3ME.Business.Controllers.MasterController.REFERRER_URL_KEY";

        protected readonly IApplicationRepository AppRepository = null;
        protected readonly ITweetRepository TweetRepository = null;
        protected readonly ITweeterRepository TweeterRepository = null;
        protected readonly ILanguageRepository LanguageRepository = null;

        public MasterController(IApplicationRepository appRepository, ITweetRepository tweetRepository,
            ITweeterRepository tweeterRepository, ILanguageRepository languageRepository)
        {
            this.AppRepository = appRepository;
            this.TweetRepository = tweetRepository;
            this.TweeterRepository = tweeterRepository;
            this.LanguageRepository = languageRepository;
        }

        private Application _app = null;
        protected Application App
        {
            get
            {
                if (_app == null)
                {
                    string host = Request.ServerVariables["HTTP_HOST"];
                    host = Regex.Replace(host, @"(.*?)\:[0-9]+", "$1");
                    _app = AppRepository.GetApplicationFromUrl(host);
                }

                return _app;
            }
        }

        private Tweeter _tweeter = null;
        protected Tweeter Tweeter
        {
            get
            {
                if (_tweeter == null)
                {
                    if (Session["TweeterId"] != null)
                    {
                        long tweeterId = (long)Session["TweeterId"];

                        _tweeter = TweeterRepository.FromId(tweeterId);
                    }
                }

                return _tweeter;
            }
            set
            {
                _tweeter = value;

                if (_tweeter != null)
                {
                    Session.Add("TweeterId", _tweeter.Id);
                }
                else
                {
                    Session.Remove("TweeterId");
                }
            }
        }

        private Language _defaultLanguage = null;
        protected Language DefaultLanguage
        {
            get
            {
                if (_defaultLanguage == null)
                {
                    if (Tweeter != null)
                    {
                        _defaultLanguage = Tweeter.DefaultLanguage;
                    }

                    HttpCookie languageCookie = Request.Cookies["Default_Language"];
                    if (languageCookie != null)
                    {
                        int languageId = Int32.TryParse(languageCookie.Value, out languageId) ? languageId : -1;
                        _defaultLanguage = LanguageRepository.FromId(languageId);
                    }
                }

                return _defaultLanguage;
            }
            set
            {
                _defaultLanguage = value;

                if (Tweeter != null && Tweeter.DefaultLanguage != value)
                {
                    Tweeter.DefaultLanguage = value;
                    TweeterRepository.Save(Tweeter);
                }

                HttpCookie languageCookie = new HttpCookie("Default_Language");
                languageCookie.Value = value != null ? value.Id.ToString() : "";
                languageCookie.Expires = value != null ? DateTime.UtcNow.AddDays(30.0) : DateTime.UtcNow.AddYears(-9);
                Response.Cookies.Add(languageCookie);
            }
        }

        protected void StoreTempReferrerUrl()
        {
            Uri url = Request.Url;
            string webAddress = url.Scheme + "://" + url.Host + (url.Port != 80 ? ":" + url.Port : "");

            if (Request.UrlReferrer != null)
            {
                string referrerUrl = Request.UrlReferrer.ToString();

                if (referrerUrl.StartsWith(webAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    TempData[REFERRER_URL_KEY] = referrerUrl;
                }
            }
        }

        protected string GetTempReferrerUrl()
        {
            return TempData[REFERRER_URL_KEY] as string;
        }

        protected string ReturnUrl
        {
            get { return Request["returnUrl"] ?? "~/"; }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Tweeter == null)
            {
                HttpCookie loginCookie = Request.Cookies["Login_Cookie"];
                if (loginCookie != null)
                {
                    loginCookie.Expires = DateTime.UtcNow.AddYears(-9);

                    int id = Int32.TryParse(loginCookie.Values["ID"], out id) ? id : -1;
                    string code = (loginCookie.Values["Code"] ?? "").Trim();

                    if (code.Length > 0)
                    {
                        Tweeter foundTweeter = TweeterRepository.FromTwitterId(id);

                        if (foundTweeter != null)
                        {
                            // Expire old logins
                            IList<PersistentLogin> loginsToRemove = foundTweeter.PersistentLogins
                                .Where(pl => pl.LastLoginDate < DateTime.UtcNow.AddDays(-30.0)).ToList();
                            foundTweeter.PersistentLogins.RemoveAll(loginsToRemove);

                            // Find a login that matches the key
                            PersistentLogin foundLogin = foundTweeter.PersistentLogins
                                .FirstOrDefault(pl => pl.SecureKey == code);

                            if (foundLogin != null)
                            {
                                // Generate new code
                                string newCode = SecurityUtils.GenerateSecureKey(KeyStrength._512bit);

                                // Set cookie
                                loginCookie.Values["Code"] = newCode;
                                loginCookie.Expires = DateTime.UtcNow.AddDays(30.0);
                                loginCookie.HttpOnly = true;

                                // Update login entity
                                foundLogin.SecureKey = newCode;
                                foundLogin.LastLoginDate = DateTime.UtcNow;

                                // Assign Tweeter as logged in user
                                Tweeter = foundTweeter;
                            }

                            // Update user in database
                            TweeterRepository.Save(foundTweeter);
                        }
                    }

                    Response.Cookies.Add(loginCookie);
                }
            }

            LanguageSelectForm languageForm = new LanguageSelectForm();
            languageForm.Languages = LanguageRepository.ListLanguages(false);
            languageForm.SelectedLanguage = DefaultLanguage != null ? (int?)DefaultLanguage.Id : null;

            IList<TagReport> tags = TweetRepository.MostPopularTags(App, DefaultLanguage, 30).OrderBy(t => t.Tag).ToList();

            long lowestUsageCount = tags.Count > 0 ? tags.Min(t => t.UsageCount) : 0;
            long highestUsageCount = tags.Count > 0 ? tags.Max(t => t.UsageCount) : 0;

            TagCloudView tagCloud = new TagCloudView();

            foreach (TagReport tag in tags)
            {
                double weight = Math.Log((double)tag.UsageCount) / Math.Log((double)highestUsageCount) * 100.0;

                TagCloudView.TagStrenth strength;

                if (weight >= 99)
                {
                    strength = TagCloudView.TagStrenth.VeryLarge;
                }
                else if (weight >= 75)
                {
                    strength = TagCloudView.TagStrenth.Large;
                }
                else if (weight >= 50)
                {
                    strength = TagCloudView.TagStrenth.Medium;
                }
                else if (weight >= 25)
                {
                    strength = TagCloudView.TagStrenth.Small;
                }
                else
                {
                    strength = TagCloudView.TagStrenth.VerySmall;
                }

                tagCloud.Tags.Add(new TagCloudView.Tag()
                {
                    Name = tag.Tag,
                    Strenth = strength
                });
            }

            MasterFrontEndView masterView = new MasterFrontEndView(App, languageForm, tagCloud);

            if (Tweeter != null)
            {
                masterView.TweeterPanel = new TweeterPanelView()
                {
                    Username = Tweeter.Username,
                    ImageUrl = Tweeter.ImageUrl,
                    TotalVotes = TweeterRepository.TotalVotesByTweeter(Tweeter, App),
                    ApplicationName = App.Title
                };
            }

            if (DefaultLanguage != null)
            {
                masterView.DefaultLanguage = DefaultLanguage;
            }

            ViewData["MasterView"] = masterView;

            base.OnActionExecuting(filterContext);
        }
    }
}