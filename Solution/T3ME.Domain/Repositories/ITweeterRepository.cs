using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Domain.Models;
using T3ME.Domain.ReportModels;

namespace T3ME.Domain.Repositories
{
    public interface ITweeterRepository : IRepository<Tweeter>
    {
        Tweeter FromTwitterId(long twitterId);

        int TotalVotesByTweeter(Tweeter tweeter, Application app);
        int TotalTweetsFromTweeter(Tweeter tweeter, Application app);
        int TotalTweetsVotedForTweeter(Tweeter tweeter, Application app);

        IList<TweeterReport> MostPopularTweeters(Application app, Language language, int maxRecords);
        IList<TweeterReport> TweetersWithMostMostVotes(Application app, Language language, int maxRecords);

        IList<HasVotedReport> HasTweeterVotedForTweets(Application app, Tweeter tweeter, IList<long> tweetIds);
    }
}