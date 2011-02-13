using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Business.ViewModels
{
    public class StatsView
    {
        public StatsView()
        {
            MostPopualarHashtags = new List<TagView>();
        }

        public TweeterStatsView MostFrequentTweeters { get; set; }
        public TweeterStatsView TweetersWithMostVotes { get; set; }
        public IList<TagView> MostPopualarHashtags { get; protected set; }

        public int TotalTweets { get; set; }
        public int TotalVotes { get; set; }

        public bool HasSelectedLanguage { get; set; }
        public string LanguageName { get; set; }
        public int TotalTweetsForLanguage { get; set; }
        public int TotalVotesForLanguage { get; set; }
    }
}