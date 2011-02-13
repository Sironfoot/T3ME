using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Domain.Repositories;
using T3ME.Domain.Models;
using MvcLibrary.NHibernate.StrongTyping;
using MvcLibrary.NHibernate.SessionManagement;
using T3ME.Domain.ReportModels;
using NHibernate.Linq;
using NHibernate.Linq.Expressions;
using NHibernate.Transform;
using NHExpressions = NHibernate.Criterion.Expression;
using NHibernate;

namespace T3ME.Domain.NHibernate.Repositories
{
    public class TweeterRepository : NHibernateRepository<Tweeter>, ITweeterRepository
    {
        public TweeterRepository(ISessionProviderFactory sessionProviderFactory) : base(sessionProviderFactory) { }

        public Tweeter FromTwitterId(long twitterId)
        {
            return Session
                .CreateCriteria<Tweeter>()
                .Add(RestrictionsFor<Tweeter>.Eq(t => t.TwitterId, twitterId))
                .SetMaxResults(1)
                .UniqueResult<Tweeter>();
        }

        public int TotalVotesByTweeter(Tweeter tweeter, Application app)
        {
            return Session
                .Query<Vote>()
                .Count(v => v.Tweeter == tweeter && v.Tweet.App == app);
        }

        public int TotalTweetsFromTweeter(Tweeter tweeter, Application app)
        {
            return Session
                .Query<Tweet>()
                .Count(t => t.Tweeter == tweeter && t.App == app);
        }

        public IList<TweeterReport> MostPopularTweeters(Application app, Language language, int maxRecords)
        {
            IQuery query = Session
                .CreateQuery(@" select u.Username as Username, u.ImageUrl as ProfileImageUrl, count(t.Id) as TotalTweets
                                from Tweeter u
                                    join u.Tweets t
                                where t.App = :app " + (language != null ? "and t.Language = :language " : "") + @"
                                group by u.Username, u.ImageUrl
                                order by count(t.Id) desc")
                .SetParameter<Application>("app", app);

            if(language != null)
            {
                query.SetParameter<Language>("language", language);
            }

            return query.SetMaxResults(maxRecords)
                .SetResultTransformer(Transformers.AliasToBean<TweeterReport>())
                .List<TweeterReport>();
        }

        public IList<TweeterReport> TweetersWithMostMostVotes(Application app, Language language, int maxRecords)
        {
            IQuery query = Session
                .CreateQuery(@" select u.Username as Username, u.ImageUrl as ProfileImageUrl, count(v.Id) as TotalVotes
                                from Tweeter u
                                    join u.Tweets t
                                    join t.Votes v
                                where t.App = :app " + (language != null ? "and t.Language = :language " : "") + @"
                                group by u.Username, u.ImageUrl
                                order by count(v.Id) desc")
                .SetParameter<Application>("app", app);

            if(language != null)
            {
                query.SetParameter<Language>("language", language);
            }

            return query.SetMaxResults(maxRecords)
                .SetResultTransformer(Transformers.AliasToBean<TweeterReport>())
                .List<TweeterReport>();
        }

        public int TotalTweetsVotedForTweeter(Tweeter tweeter, Application app)
        {
            return (from t in Session.Query<Tweet>()
                    where t.Votes.Count > 0 &&
                        t.App == app &&
                        t.Tweeter == tweeter
                    select t).Count();
        }

        public IList<HasVotedReport> HasTweeterVotedForTweets(Application app, Tweeter tweeter, IList<long> tweetIds)
        {
            IList<HasVotedReport> votes = new List<HasVotedReport>(tweetIds.Count);

            IList<Tweet> tweets = new List<Tweet>();

            if (tweeter != null)
            {
                tweets = (from t in Session.Query<Tweet>()
                          where t.App == app &&
                              tweetIds.Contains(t.TwitterId) &&
                              t.Votes.Any(v => v.Tweeter == tweeter)
                          select t).ToList();
            }

            foreach (long tweetId in tweetIds)
            {
                HasVotedReport vote = new HasVotedReport()
                {
                    TwitterId = tweetId,
                    HasVoted = tweets.Any(t => t.TwitterId == tweetId)
                };

                votes.Add(vote);
            }

            return votes;
        }
    }
}