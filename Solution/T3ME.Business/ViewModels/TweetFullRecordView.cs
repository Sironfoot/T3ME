using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Business.ViewModels
{
    public class TweetFullRecordView : TweetView
    {
        public class Voter
        {
            public string Username { get; set; }
            public string ProfileImageUrl { get; set; }
        }

        public TweetFullRecordView()
        {
            this.Voters = new List<Voter>();
        }

        public string FullName { get; set; }
        public int TotalTweetsByTweeter { get; set; }
        public IList<Voter> Voters { get; protected set; }
    }
}