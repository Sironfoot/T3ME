using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twitterizer;
using T3ME.Business.Controllers;
using T3ME.Domain.Repositories;
using T3ME.Business.Utils;
using T3ME.Domain.Models;
using T3ME.Business.ViewModels;
using MvcLibrary.Utils;
using AutoMapper;

namespace T3ME.Web.Controllers
{
    public class OAuthController : MasterController
    {
        public OAuthController(IApplicationRepository appRepository, ITweetRepository tweetRepository,
            ITweeterRepository tweeterRepository, ILanguageRepository languageRepository)
            : base(appRepository, tweetRepository, tweeterRepository, languageRepository) { }

        public object Signin(bool popup = false)
        {
            string host = Request.ServerVariables["HTTP_HOST"];
            string callbackUrl = Request.Url.Scheme + "://" + host + "/OAuth/Callback" +(popup ? "?popup=true" : "");

            OAuthTokenResponse authorizationTokens = OAuthUtility.GetRequestToken(
                App.ConsumerKey, App.ConsumerSecret, callbackUrl);

            string authorizeUrl = OAuthUtility.BuildAuthorizationUri(authorizationTokens.Token).ToString();
            string authenticationUrl = authorizeUrl.Replace("authorize", "authenticate") +"&force_login=true";

            StoreTempReferrerUrl();

            if (Request.IsAjaxRequest())
            {
                return authenticationUrl;
            }
            else
            {
                return Redirect(authenticationUrl);
            }
        }

        public ActionResult Callback(string oauth_token, string oauth_verifier, bool popup = false)
        {
            OAuthTokenResponse tokens = OAuthUtility.GetAccessToken(
                App.ConsumerKey, App.ConsumerSecret, oauth_token, oauth_verifier);

            OAuthTokens oauthTokens = new OAuthTokens();
            oauthTokens.AccessToken = tokens.Token;
            oauthTokens.AccessTokenSecret = tokens.TokenSecret;
            oauthTokens.ConsumerKey = App.ConsumerKey;
            oauthTokens.ConsumerSecret = App.ConsumerSecret;

            TwitterUser user = TwitterUser.Show(oauthTokens, tokens.UserId).ResponseObject;

            Tweeter tweeter = TweeterRepository.FromTwitterId((long)user.Id);

            if (tweeter != null)
            {
                tweeter = Mapper.Map<TwitterUser, Tweeter>(user, tweeter);
            }
            else
            {
                tweeter = Mapper.Map<TwitterUser, Tweeter>(user);
                TweeterRepository.Save(tweeter);
            }

            string secureCode = SecurityUtils.GenerateSecureKey(KeyStrength._512bit);

            HttpCookie loginCookie = new HttpCookie("Login_Cookie");

            loginCookie.Values["ID"] = tweeter.TwitterId.ToString();
            loginCookie.Values["Code"] = secureCode;
            loginCookie.Expires = DateTime.UtcNow.AddDays(30.0);
            loginCookie.HttpOnly = true;

            Response.Cookies.Add(loginCookie);

            PersistentLogin login = new PersistentLogin(tweeter);
            login.SecureKey = secureCode;
            login.LastLoginDate = DateTime.UtcNow;
            tweeter.PersistentLogins.Add(login);

            TweeterRepository.Save(tweeter);

            Tweeter = tweeter;

            if (!popup)
            {
                string referrerUrl = GetTempReferrerUrl();
                if (!String.IsNullOrWhiteSpace(referrerUrl))
                {
                    return Redirect(referrerUrl);
                }

                return RedirectToAction("Latest", "Display");
            }
            else
            {
                return View("Complete", Tweeter);
            }
        }

        [HttpPost]
        public ActionResult Signout()
        {
            HttpCookie loginCookie = Request.Cookies["Login_Cookie"];
            if (loginCookie != null)
            {
                if (Tweeter != null)
                {
                    string secureKey = loginCookie.Values["Code"];

                    IList<PersistentLogin> logins = TweeterRepository.Query<PersistentLogin>()
                        .Where(pl => pl.Tweeter == Tweeter && pl.SecureKey == secureKey)
                        .ToList();

                    Tweeter.PersistentLogins.RemoveAll(logins);
                    TweeterRepository.Save(Tweeter);
                }

                loginCookie.Expires = DateTime.UtcNow.AddYears(-9);
                Response.Cookies.Add(loginCookie);
            }

            Tweeter = null;

            if (Request.UrlReferrer != null && Request.UrlReferrer.Host == App.Url)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

            return RedirectToAction("Latest", "Display");
        }

        public ActionResult CurrentUserDetail()
        {
            TweeterPanelView tweeterPanel = null;

            if (Tweeter != null)
            {
                tweeterPanel = new TweeterPanelView()
                {
                    Username = Tweeter.Username,
                    ImageUrl = Tweeter.ImageUrl,
                    TotalVotes = TweeterRepository.TotalVotesByTweeter(Tweeter, App),
                    ApplicationName = App.Title
                };
            }

            return PartialView("TweeterPanel", tweeterPanel);
        }

        //public ActionResult ReadWriteToken()
        //{
        //    Uri url = Request.Url;

        //    string callbackUrl = url.Scheme + "://" + url.Host + (url.Port != 80 ? ":" + url.Port : "") + "/OAuth/ReadWriteTokenCallback";

        //    OAuthTokenResponse authorizationTokens = OAuthUtility.GetRequestToken(
        //        MagicValues.OAuthConsumerKey, MagicValues.OAuthConsumerSecret, callbackUrl);

        //    string authorizeUrl = OAuthUtility.BuildAuthorizationUri(authorizationTokens.Token).ToString();

        //    return Redirect(authorizeUrl);
        //}

        //public string ReadWriteTokenCallback(string oauth_token, string oauth_verifier)
        //{
        //    OAuthTokenResponse tokens = OAuthUtility.GetAccessToken(
        //        MagicValues.OAuthConsumerKey, MagicValues.OAuthConsumerSecret, oauth_token, oauth_verifier);

        //    return "Access Token: " + tokens.Token + "<br />" +
        //           "Token Secret: " + tokens.TokenSecret;
        //}
    }
}