using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Business.Utils
{
    public static class MagicValues
    {
        public static readonly string OAuthConsumerKey = "8Q41XjtAXhXBpi6PWeWNA";
        public static readonly string OAuthConsumerSecret = "J6TcHKHyBmGakQnNW6TaQ5HOhHD1ErtfY01p1O8YYo";

        /// <summary>
        ///     Records per page to display for Tweets output
        /// </summary>
        public static readonly int RecordsPerPage = 20;

        /// <summary>
        ///     Number of votes required for a tweet before it is retweeted
        ///     by the Twitter account for the Application
        /// </summary>
        public static readonly int NumVotesForRetweeting = 2;

        /// <summary>
        ///     The maximum number of tweets that will be persisted per Application
        ///     before the oldest tweets are deleted.
        /// </summary>
        public static readonly int MaxTweetsPerApplication = 100000;


        /// <summary>
        ///     Maximum number of allowed Twitter API calls allowed per hour,
        ///     other known as 'Rate limiting'
        /// </summary>
        public static readonly int MaxApiHitsPerHour = 100;

        /// <summary>
        ///     Follow a Tweeter when this many of their tweets receive a vote.
        /// </summary>
        public static readonly int NumTweetsVotedForBeforeFollowing = 3;
    }
}