using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Domain.Models;
using T3ME.Domain.Repositories;
using MvcLibrary.NHibernate.SessionManagement;
using MvcLibrary.NHibernate.StrongTyping;
using MvcLibrary.Utils.Collections;
using NHibernate;
using NHibernate.Criterion;
using System.Linq.Expressions;
using T3ME.Domain.ReportModels;
using NHibernate.Transform;
using NHExpressions = NHibernate.Criterion.Expression;
using NHibernate.SqlCommand;
using NHibernate.Linq;
using NHibernate.Linq.Expressions;
using System.Collections;
using T3ME.Domain.Queries;

namespace T3ME.Domain.NHibernate.Repositories
{
    public class TweetRepository : NHibernateRepository<Tweet>, ITweetRepository
    {
        public TweetRepository(ISessionProviderFactory sessionProviderFactory) : base(sessionProviderFactory) { }

        public Tweet FromTwitterId(long id, Application app)
        {
            return Session
                .CreateCriteria<Tweet>()
                .Add(RestrictionsFor<Tweet>.Eq(t => t.TwitterId, id))
                .Add(RestrictionsFor<Tweet>.Eq(t => t.App, app))
                .SetMaxResults(1)
                .UniqueResult<Tweet>();
        }

        public Tweet LatestTweet(Application app)
        {
            return Session
                .CreateCriteria<Tweet>()
                .Add(RestrictionsFor<Tweet>.Eq(t => t.App, app))
                .AddOrder(OrderFor<Tweet>.Desc(t => t.PostedDate))
                .SetMaxResults(1)
                .UniqueResult<Tweet>();
        }

        public void DeleteOldestAfter(int numTweets, Application app)
        {
            if (app.Tweets.Count > numTweets)
            {
                using (ITransaction transaction = Session.BeginTransaction())
                {
                    var tweetsToDelete = (from t in Session.Query<Tweet>()
                                          where t.App == app
                                             && t.Votes.Count == 0
                                          orderby t.PostedDate descending
                                          select t).Skip(numTweets);

                    foreach (Tweet tweet in tweetsToDelete)
                    {
                        Session.Delete(tweet);
                    }

                    transaction.Commit();
                }
            }
        }

        public IPagedList<TweetReport> Search(Application app, int pageNumber, int recordsPerPage, Tweeter tweeter,
            string search, bool votedOnly, Language language, SearchOrder order, long? sinceTwitterId)
        {
            int startIndex = (pageNumber - 1) * recordsPerPage;

            string countHql = "select count(distinct t) as TweetCount ";

            string resultsHql = @"select t.Id as Id,
                                        t.TwitterId as TwitterId,
                                        u.Username as Username,
                                        u.ImageUrl as ProfileImageUrl,
                                        t.Message as Message,
                                        t.PostedDate as DatePosted,
                                        t.Device.Name as DeviceName,
                                        t.Device.Url as DeviceUrl,
                                        count(distinct v) as TotalVotes ";

            if (tweeter != null)
            {
                resultsHql += ", (select count(v1.Id) from t.Votes v1 where v1.Tweeter = :tweeter) as HasBeenVotedByUser ";
            }

            string mainQuery = @"from Tweet t
                                    join t.Tweeter u
                                    left join t.Votes v
                                    left join t.Tags tag
                                    left join t.Language lang
                                 where t.App = :app ";

            if (language != null)
            {
                mainQuery += @"and lang = :language ";
            }

            if (sinceTwitterId.HasValue)
            {
                if (order == SearchOrder.Latest)
                {
                    mainQuery += "and t.PostedDate > (select it.PostedDate from Tweet it where it.TwitterId = :sinceTwitterId) ";
                }
                else if (order == SearchOrder.RecentlyVoted)
                {
                    mainQuery += "and t.FirstVoteCast > (select it.FirstVoteCast from Tweet it where it.TwitterId = :sinceTwitterId) ";
                }
            }

            string usernameQuery = null;
            string tagQuery = null;
            string messageQuery = null;

            if (!String.IsNullOrWhiteSpace(search))
            {
                if (search.Trim().StartsWith("@"))
                {
                    mainQuery += "and u.Username = :username ";
                    usernameQuery = search.TrimStart(' ', '@');
                }
                else if (search.Trim().StartsWith("#"))
                {
                    mainQuery += "and tag.Text = :tag ";
                    tagQuery = search.TrimStart(' ', '#');
                }
                else
                {
                    mainQuery += "and t.Message like :message ";
                    messageQuery = search;
                }
            }

            countHql += mainQuery;

            resultsHql += mainQuery +
                "group by t.Id, t.TwitterId, u.Username, u.ImageUrl, t.Message, t.PostedDate, t.Device.Name, t.Device.Url, t.FirstVoteCast ";

            if (votedOnly)
            {
                string having = "having size(t.Votes) > 0 ";
                resultsHql += having;

                countHql += "and size(t.Votes) > 0 ";
            }

            if (order == SearchOrder.Latest)
            {
                resultsHql += "order by t.PostedDate desc ";
            }
            else if (order == SearchOrder.RecentlyVoted)
            {
                resultsHql += "order by t.FirstVoteCast desc ";
            }
            else if (order == SearchOrder.TopRated)
            {
                resultsHql += "order by count(distinct v) desc ";
            }

            IQuery resultsQuery = Session
                .CreateQuery(resultsHql)
                .SetParameter<Application>("app", app);
                
            IQuery countQuery = Session
                .CreateQuery(countHql)
                .SetParameter<Application>("app", app);

            if (usernameQuery != null)
            {
                countQuery.SetParameter<string>("username", usernameQuery);
                resultsQuery.SetParameter<string>("username", usernameQuery);
            }
            if (tagQuery != null)
            {
                countQuery.SetParameter<string>("tag", tagQuery);
                resultsQuery.SetParameter<string>("tag", tagQuery);
            }
            if (messageQuery != null)
            {
                countQuery.SetParameter<string>("message", "%" + messageQuery + "%");
                resultsQuery.SetParameter<string>("message", "%" + messageQuery + "%");
            }

            if (tweeter != null)
            {
                resultsQuery.SetParameter<Tweeter>("tweeter", tweeter);
            }

            if (language != null)
            {
                countQuery.SetParameter<Language>("language", language);
                resultsQuery.SetParameter<Language>("language", language);
            }

            if (sinceTwitterId.HasValue)
            {
                countQuery.SetParameter<long>("sinceTwitterId", sinceTwitterId.Value);
                resultsQuery.SetParameter<long>("sinceTwitterId", sinceTwitterId.Value);
            }

            int totalRecords = (int)countQuery.UniqueResult<long>();

            IList<TweetReport> pagedTweets = resultsQuery
                .SetFirstResult(startIndex)
                .SetMaxResults(recordsPerPage)
                .SetResultTransformer(new CustomTransformer())
                .List<TweetReport>();

            return new PagedList<TweetReport>(pagedTweets, recordsPerPage, pageNumber, totalRecords);
        }

        public class CustomTransformer : IResultTransformer
        {
            public IList TransformList(IList collection)
            {
                return collection;
            }

            public object TransformTuple(object[] tuple, string[] aliases)
            {
                TweetReport tweet = new TweetReport();
                Type tweetType = typeof(TweetReport);

                for (int i = 0; i < aliases.Length; i++)
                {
                    string alias = aliases[i];
                    object value = tuple[i];

                    if (alias == "HasBeenVotedByUser")
                    {
                        long count = (long)value;

                        if (count > 0)
                        {
                            tweet.HasBeenVotedByUser = true;
                        }
                    }
                    else
                    {
                        tweetType.GetProperty(alias).SetValue(tweet, value, null);
                    }
                }

                return tweet;
            }
        }

        public IList<TagReport> MostPopularTags(Application app, Language language, int maxRecords)
        {
            string queryString = @" select t.Text as Tag, count(t.Id) as UsageCount
                                    from Tag t
                                        join t.Tweet tw
                                        left join tw.Language lang
                                    where tw.App = :app and
                                        t.Text not in (:ignoredTags) ";

            if(language != null)
            {
                queryString += @"       and lang = :language ";
            }

            queryString += @"       group by t.Text
                                    order by count(t.Id) desc";

            IQuery query = Session
                .CreateQuery(queryString)
                .SetParameter<Application>("app", app)
                .SetParameterList("ignoredTags", app.HashTags.ToArray());

            if(language != null)
            {
                query = query.SetParameter<Language>("language", language);
            }

            return query.SetMaxResults(maxRecords)
                .SetResultTransformer(Transformers.AliasToBean<TagReport>())
                .List<TagReport>();
        }

        public IPagedList<Tweeter> GetVotersForTweet(Tweet tweet, int pageNumber, int recordsPerPage)
        {
            int startIndex = (pageNumber - 1) * recordsPerPage;

            int totalVotes = tweet.Votes.Count;

            var tweeters = (from v in tweet.Votes
                            orderby v.VotedDate descending
                            select v.Tweeter)
                           .Skip(startIndex)
                           .Take(recordsPerPage)
                           .ToList();

            return new PagedList<Tweeter>(tweeters, recordsPerPage, pageNumber, totalVotes);
        }

        public int TotalTweets(Application app, Language language)
        {
            var query = Session.Query<Tweet>()
                .Where(t => t.App == app);

            if (language != null)
            {
                query = query.Where(t => t.Language == language);
            }

            return query.Count();
        }

        public int TotalVotes(Application app, Language language)
        {
            var query = Session.Query<Vote>()
                .Where(v => v.Tweet.App == app);

            if (language != null)
            {
                query = query.Where(v => v.Tweet.Language == language);
            }

            return query.Count();
        }
    }
}