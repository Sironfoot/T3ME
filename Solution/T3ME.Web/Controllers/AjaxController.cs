using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using T3ME.Business.Controllers;
using T3ME.Domain.Repositories;
using T3ME.Business.ViewModels;
using T3ME.Domain.ReportModels;
using T3ME.Domain.Models;
using T3ME.Domain.Queries;
using MvcLibrary.Utils.Collections;

namespace T3ME.Web.Controllers
{
    public class AjaxController : MasterController
    {
        public AjaxController(IApplicationRepository appRepository, ITweetRepository tweetRepository,
            ITweeterRepository tweeterRepository, ILanguageRepository languageRepository)
            : base(appRepository, tweetRepository, tweeterRepository, languageRepository) { }

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

        public class JsonVoteReport
        {
            public string TwitterId { get; set; } // Long Twitter ID's can't be read by JavaScript
            public bool HasVoted { get; set; }
        }

        public JsonResult CheckTweetsForVotes(string tweetIds)
        {
            IList<JsonVoteReport> votes = new List<JsonVoteReport>();

            if (tweetIds != null)
            {
                IList<long> ids = tweetIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Int64.Parse(t)).ToList();
                foreach (HasVotedReport report in TweeterRepository.HasTweeterVotedForTweets(App, Tweeter, ids))
                {
                    votes.Add(new JsonVoteReport()
                    {
                        TwitterId = report.TwitterId.ToString(),
                        HasVoted = report.HasVoted
                    });
                }
            }

            return Json(votes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewTweetsSince(long lastTweetId, string search, SearchOrder? order)
        {
            SearchOrder searchOrder = order.GetValueOrDefault(SearchOrder.Latest);

            IPagedList<TweetReport> tweets = TweetRepository.Search(App, 1, 20, Tweeter, search,
                searchOrder != SearchOrder.Latest, DefaultLanguage, searchOrder, lastTweetId);

            TweetViewList tweetList = new TweetViewList("", tweets.TotalItems, tweets.PageNumber, tweets.ItemsPerPage);
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

            return PartialView("Tweet", tweetList);
        }
    }
}