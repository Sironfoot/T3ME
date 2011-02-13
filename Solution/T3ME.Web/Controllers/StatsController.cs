using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using T3ME.Domain.Repositories;
using T3ME.Business.Controllers;
using T3ME.Domain.ReportModels;
using T3ME.Business.ViewModels;

namespace T3ME.Web.Controllers
{
    public class StatsController : MasterController
    {
        public StatsController(IApplicationRepository appRepository, ITweetRepository tweetRepository,
            ITweeterRepository tweeterRepository, ILanguageRepository languageRepository)
            : base(appRepository, tweetRepository, tweeterRepository, languageRepository) { }

        public ActionResult Index()
        {
            StatsView stats = new StatsView();

            TweeterStatsView mostPopularTweeters = new TweeterStatsView();
            mostPopularTweeters.UsernameLink = "/";
            mostPopularTweeters.CountLabel = "tweets";
            foreach (TweeterReport tweeter in TweeterRepository.MostPopularTweeters(App, DefaultLanguage, 20))
            {
                mostPopularTweeters.Tweeters.Add(new TweeterStatsView.Tweeter()
                {
                    Username = tweeter.Username,
                    ImageUrl = tweeter.ProfileImageUrl,
                    ItemCount = tweeter.TotalTweets
                });
            }

            stats.MostFrequentTweeters = mostPopularTweeters;

            TweeterStatsView tweetersMostVotes = new TweeterStatsView();
            tweetersMostVotes.UsernameLink = "/top-rated";
            tweetersMostVotes.CountLabel = "votes";
            foreach (TweeterReport tweeter in TweeterRepository.TweetersWithMostMostVotes(App, DefaultLanguage, 20))
            {
                tweetersMostVotes.Tweeters.Add(new TweeterStatsView.Tweeter()
                {
                    Username = tweeter.Username,
                    ImageUrl = tweeter.ProfileImageUrl,
                    ItemCount = tweeter.TotalVotes
                });
            }

            stats.TweetersWithMostVotes = tweetersMostVotes;

            foreach (TagReport tagReport in TweetRepository.MostPopularTags(App, DefaultLanguage, 30))
            {
                TagView tagView = new TagView()
                {
                    Name = tagReport.Tag,
                    UsageCount = (int)tagReport.UsageCount
                };

                stats.MostPopualarHashtags.Add(tagView);
            }

            stats.TotalTweets = TweetRepository.TotalTweets(App, null);
            stats.TotalVotes = TweetRepository.TotalVotes(App, null);

            if (DefaultLanguage != null)
            {
                stats.HasSelectedLanguage = true;
                stats.LanguageName = DefaultLanguage.Name;

                stats.TotalTweetsForLanguage = TweetRepository.TotalTweets(App, DefaultLanguage);
                stats.TotalVotesForLanguage = TweetRepository.TotalVotes(App, DefaultLanguage);
            }

            return View(stats);
        }
    }
}