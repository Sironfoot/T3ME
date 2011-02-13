using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Domain.Models;
using MvcLibrary.Utils.Collections;
using System.Linq.Expressions;
using T3ME.Domain.ReportModels;
using T3ME.Domain.Queries;

namespace T3ME.Domain.Repositories
{
    public interface ITweetRepository : IRepository<Tweet>
    {
        Tweet FromTwitterId(long id, Application app);
        Tweet LatestTweet(Application app);
        void DeleteOldestAfter(int numTweets, Application app);

        IPagedList<TweetReport> Search(Application app, int pageNumber, int recordsPerPage,
            Tweeter tweeter, string search, bool votedOnly, Language language, SearchOrder order, long? sinceTwitterId);

        IList<TagReport> MostPopularTags(Application app, Language language, int maxRecords);

        IPagedList<Tweeter> GetVotersForTweet(Tweet tweet, int pageNumber, int recordsPerPage);

        int TotalTweets(Application app, Language language);
        int TotalVotes(Application app, Language language);
    }
}