using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Domain.ReportModels
{
    public class TweeterReport
    {
        public string Username { get; set; }
        public string ProfileImageUrl { get; set; }
        public long TotalTweets { get; set; }
        public long TotalVotes { get; set; }
    }
}