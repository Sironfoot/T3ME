using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Business.ViewModels
{
    public class TweeterStatsView
    {
        public class Tweeter
        {
            public string Username { get; set; }
            public string ImageUrl { get; set; }
            public long ItemCount { get; set; }
        }

        public TweeterStatsView()
        {
            Tweeters = new List<TweeterStatsView.Tweeter>();
        }

        public string UsernameLink { get; set; }
        public string CountLabel { get; set; }

        public IList<TweeterStatsView.Tweeter> Tweeters { get; protected set; }
    }
}