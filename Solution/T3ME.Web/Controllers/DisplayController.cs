using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using T3ME.Domain.Models;
using MvcLibrary.Utils.Collections;
using T3ME.Domain.Repositories;
using T3ME.Domain.Models.Components;
using T3ME.Business.Controllers;
using T3ME.Business.Utils;
using System.Linq.Expressions;
using MvcLibrary.ActionResults;
using T3ME.Domain.ReportModels;
using T3ME.Business.ViewModels;
using MvcLibrary.Utils;
using System.Net;
using T3ME.Domain.Queries;
using System.Threading;
using Twitterizer;
using AutoMapper;

namespace T3ME.Web.Controllers
{
    [HandleError]
    [ValidateInput(false)]
    public class DisplayController : MasterController
    {
        public DisplayController(IApplicationRepository appRepository, ITweetRepository tweetRepository,
            ITweeterRepository tweeterRepository, ILanguageRepository languageRepository)
            : base(appRepository, tweetRepository, tweeterRepository, languageRepository) { }

        public ActionResult Status(long twitterId, string message)
        {
            Tweet tweet = TweetRepository.FromTwitterId(twitterId, App);

            string urlMessage = (message ?? "").Trim();
            string realMessage = tweet.Message.ToFriendlyUrl();

            if (urlMessage != realMessage)
            {
                string redirectUrl = Url.Action("Status", new { twitterId = twitterId, message = realMessage });
                return new PermanentRedirectResult(redirectUrl);
            }

            TweetFullRecordView tweetView = new TweetFullRecordView()
            {
                Id = tweet.Id,
                TwitterId = tweet.TwitterId,
                Username = tweet.Tweeter.Username,
                ProfileImageUrl = tweet.Tweeter.ImageUrl,
                Message = tweet.Message,
                DatePosted = tweet.PostedDate,
                DeviceName = tweet.Device.Name,
                DeviceUrl = tweet.Device.Url,
                TotalVotes = tweet.Votes.Count,
                FullName = tweet.Tweeter.FullName,
                HasBeenVotedByUser = Tweeter != null ? tweet.Votes.Any(v => v.Tweeter == Tweeter) : false
            };

            tweetView.TotalTweetsByTweeter = TweeterRepository.TotalTweetsFromTweeter(tweet.Tweeter, App);

            IPagedList<Tweeter> voters = TweetRepository.GetVotersForTweet(tweet, 1, 10);

            foreach (Tweeter voter in voters)
            {
                tweetView.Voters.Add(new TweetFullRecordView.Voter()
                {
                    Username = voter.Username,
                    ProfileImageUrl = voter.ImageUrl
                });
            }

            return View(tweetView);
        }

        
        public ActionResult Latest(DisplayFormat format, int? page, string search)
        {
            // TODO: Remove title
            string title = "Latest " + HttpUtility.HtmlEncode(App.Noun.Plural.CapitaliseFirstLetter()) + " from Twitter";
            return Display(title, format, SearchOrder.Latest, false, page, search);
        }

        public ActionResult RecentlyVoted(DisplayFormat format, int? page, string search)
        {
            // TODO: Remove title
            string title = "Recently Voted for " + HttpUtility.HtmlEncode(App.Noun.Plural.CapitaliseFirstLetter());
            return Display(title, format, SearchOrder.RecentlyVoted, true, page, search);
        }

        public ActionResult TopRated(DisplayFormat format, int? page, string search)
        {
            // TODO: Remove title
            string title = "Top Rated " + HttpUtility.HtmlEncode(App.Noun.Plural.CapitaliseFirstLetter());
            return Display(title, format, SearchOrder.TopRated, true, page, search);
        }

        protected ActionResult Display(string pageTitle, DisplayFormat format, SearchOrder order, bool votedOnly, int? page, string search)
        {
            int pageNumber = page.GetValueOrDefault(1);

            if (format == DisplayFormat.Rss)
            {
                pageNumber = 1;
            }

            IPagedList<TweetReport> tweets = TweetRepository.Search(App, pageNumber, 20, Tweeter, search, votedOnly, DefaultLanguage, order, null);

            TweetViewList tweetList = new TweetViewList(pageTitle, tweets.TotalItems, tweets.PageNumber, tweets.ItemsPerPage);
            tweetList.SearchTerm = String.IsNullOrWhiteSpace(search) ? null : search;

            foreach (TweetReport tweet in tweets)
            {
                TweetView tweetView = new TweetView()
                {
                    Id = tweet.Id,
                    TwitterId = tweet.TwitterId,
                    Username = tweet.Username,
                    ProfileImageUrl = tweet.ProfileImageUrl,
                    Message = tweet.Message,
                    DatePosted = tweet.DatePosted,
                    DeviceName = tweet.DeviceName,
                    DeviceUrl = tweet.DeviceUrl,
                    TotalVotes = (int)tweet.TotalVotes,
                    HasBeenVotedByUser = tweet.HasBeenVotedByUser
                };

                tweetList.Tweets.Add(tweetView);
            }

            if (format == DisplayFormat.Html)
            {
                return View(tweetList);
            }
            else if (format == DisplayFormat.HtmlSnippet)
            {
                return PartialView("Tweet", tweetList);
            }
            else if (format == DisplayFormat.Json)
            {
                return Json(tweetList, JsonRequestBehavior.AllowGet);
            }
            else if (format == DisplayFormat.Rss)
            {
                string rssTitle = pageTitle;

                if (!String.IsNullOrWhiteSpace(search))
                {
                    rssTitle += " Containing \"" + search + "\"";
                }

                rssTitle += " - " + App.Title;

                RssResult<TweetView> rssResult = new RssResult<TweetView>(rssTitle, "http://" + App.Url, App.Blurb);

                rssResult.DataSource = tweetList.Tweets;

                Uri requestUrl = Request.Url;
                string baseUrl = "http://" + requestUrl.Host + (requestUrl.Port != 80 ? ":" + requestUrl.Port : "");

                rssResult.SetDataSourceFields(t => t.Message, t => baseUrl + Url.Action("Status", new { twitterId = t.TwitterId, message = t.Message.ToFriendlyUrl() }),
                    t => "@" + t.Username + " tweeted \"" + t.Message + "\"", t => "@" + t.Username, t => t.DatePosted);

                return rssResult;
            }
            else
            {
                throw new NotSupportedException(format + " is not supported");
            }
        }

        public ActionResult NotFound404(string url)
        {
            int total404Tweets = App.NotFoundTweets.Count;
            if (total404Tweets > 0)
            {
                string tweet = App.NotFoundTweets.ToList()[new Random().Next(0, total404Tweets)];
                ViewData["404Tweet"] = tweet;
            }
            else
            {
                ViewData["404Tweet"] = "Sorry, we couldn't find the page you're looking for.";
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View();
        }
    }
}