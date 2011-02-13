using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Domain.ReportModels
{
    public class TweetReport
    {
        public long Id { get; set; }
        public long TwitterId { get; set; }
        public string Username { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Message { get; set; }
        public DateTime DatePosted { get; set; }
        public string DeviceName { get; set; }
        public string DeviceUrl { get; set; }
        public long TotalVotes { get; set; }
        public bool HasBeenVotedByUser { get; set; }
    }
}