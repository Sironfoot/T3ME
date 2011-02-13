using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcLibrary.Utils;
using System.Threading;
using System.IO;
using T3ME.Domain.Models;
using T3ME.Domain.Repositories;
using T3ME.Domain.Models.Components;
using System.Web;
using Twitterizer;
using T3ME.Business.Utils;
using T3ME.Domain.Models.Enums;
using AutoMapper;
using T3ME.Business.Mapping;

namespace T3ME.Business.BackgroundTasks
{
    public class TrawlTweets : Scheduler<TrawlTweets>
    {
        protected TrawlTweets() { }

        public ITweetRepository TweetRepository { get; set; }
        public IApplicationRepository ApplicationRepository { get; set; }
        public ITweeterRepository TweeterRepository { get; set; }
        public ILanguageRepository LanguageRepository { get; set; }

        protected IList<Language> Languages = null;

        // Eg. Facebook and LinkedIn hashtags
        protected List<string> IgnoredHashtags = new List<string>() { "in", "fb" };

        protected override void Execute(DateTime runTime)
        {
            IList<Application> apps = ApplicationRepository.ListApplications();

            if (apps.Count > 0)
            {
                int requestsPerHourPerApp = (int)Math.Floor((double)MagicValues.MaxApiHitsPerHour / (double)apps.Count);
                int intervalMinutes = (int)Math.Ceiling(60.0 / (double)requestsPerHourPerApp);

                base.Interval = TimeSpan.FromMinutes(intervalMinutes);

                Languages = LanguageRepository.ListLanguages(true);

                foreach (Application app in apps)
                {
                    OAuthTokens token = new OAuthTokens();
                    token.ConsumerKey = MagicValues.OAuthConsumerKey;
                    token.ConsumerSecret = MagicValues.OAuthConsumerSecret;
                    token.AccessToken = app.Account.AccessToken;
                    token.AccessTokenSecret = app.Account.TokenSecret;
                    
                    int totalLiveTweets = app.Tweets.Count;

                    if (app.HashTags.Count > 0)
                    {
                        Tweet latestTweet = TweetRepository.LatestTweet(app);

                        StringBuilder hashSearch = new StringBuilder();
                        foreach (string hashTag in app.HashTags)
                        {
                            hashSearch.Append("#").Append(hashTag).Append(" OR ");
                        }
                        hashSearch.Remove(hashSearch.Length - 4, 4);

                        SearchOptions options = new SearchOptions();
                        options.ResultType = SearchOptionsResultType.Recent;
                        options.NumberPerPage = 100;

                        if (latestTweet != null)
                        {
                            options.SinceId = latestTweet.TwitterId;
                        }

                        TwitterResponse<TwitterSearchResultCollection> results = TwitterSearch.Search(hashSearch.ToString(), options);

                        LookupUsersOptions userLookupNames = new LookupUsersOptions();
                        foreach (TwitterSearchResult status in results.ResponseObject)
                        {
                            userLookupNames.ScreenNames.Add(status.FromUserScreenName);
                        }

                        IList<TwitterUser> users = null;

                        foreach (TwitterSearchResult status in results.ResponseObject)
                        {
                            string statusText = HttpUtility.HtmlDecode(status.Text);

                            // Ignore Retweets
                            if (statusText.IsRetweet())
                            {
                                continue;
                            }

                            // Ignore own Application's tweets
                            if (status.FromUserScreenName.Equals(app.Account.ScreenName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                continue;
                            }

                            // Ignore tweets containing own Application's username
                            if (statusText.IndexOf("@" + app.Account.ScreenName + " ", StringComparison.InvariantCultureIgnoreCase) != -1)
                            {
                                continue;
                            }

                            // Ignore tweets already indexed
                            bool exists = TweetRepository.FromTwitterId(status.Id, app) != null;
                            if (exists)
                            {
                                continue;
                            }

                            if (users == null)
                            {
                                users = userLookupNames.ScreenNames.Count > 0 ?
                                    TwitterUser.Lookup(token, userLookupNames).ResponseObject : new TwitterUserCollection();
                            }

                            Tweeter tweeter = null;

                            TwitterUser user = users
                                .FirstOrDefault(u => u.ScreenName
                                    .Equals(status.FromUserScreenName, StringComparison.InvariantCultureIgnoreCase));

                            if (user != null)
                            {
                                long twitterUserId = (long)user.Id;

                                tweeter = TweeterRepository.FromTwitterId(twitterUserId);

                                if (tweeter == null)
                                {
                                    tweeter = Mapper.Map<TwitterUser, Tweeter>(user);
                                    TweeterRepository.Save(tweeter);
                                }

                                if (!TwitterUserTweeterComparer.AreSame(user, tweeter))
                                {
                                    tweeter = Mapper.Map<TwitterUser, Tweeter>(user, tweeter);
                                    TweeterRepository.Save(tweeter);
                                }
                            }

                            if (tweeter != null)
                            {
                                // Sometimes users can constantly tweet the same message over and over again
                                // so need to filter these out
                                bool userAlreadyTweetedSameMessage = TweetRepository.Query()
                                    .Any(t => t.App == app &&
                                              t.Tweeter == tweeter &&
                                              t.Message == statusText);
                                if (userAlreadyTweetedSameMessage)
                                {
                                    continue;
                                }

                                string deviceHref = HttpUtility.HtmlDecode(status.Source);

                                Tweet tweet = new Tweet(app, tweeter);
                                tweet.TwitterId = status.Id;
                                tweet.Message = statusText;
                                tweet.PostedDate = status.CreatedDate;
                                tweet.Device = Device.FromAnchor(deviceHref);
                                tweet.Location = status.Location;

                                if (!string.IsNullOrWhiteSpace(status.Language))
                                {
                                    tweet.Language = ProcessLanguage(status.Language);
                                }

                                TwitterGeo twitterGeo = status.Geo;
                                if (twitterGeo != null)
                                {
                                    tweet.GeoInfo = new Tweet.Geo();

                                    switch (twitterGeo.ShapeType)
                                    {
                                        case TwitterGeoShapeType.CircleByCenterPoint:
                                            tweet.GeoInfo.Shape = GeoShape.CircleByCenterPoint;
                                            break;
                                        case TwitterGeoShapeType.LineString:
                                            tweet.GeoInfo.Shape = GeoShape.LineString;
                                            break;
                                        case TwitterGeoShapeType.Point:
                                            tweet.GeoInfo.Shape = GeoShape.Point;
                                            break;
                                        case TwitterGeoShapeType.Polygon:
                                            tweet.GeoInfo.Shape = GeoShape.Polygon;
                                            break;
                                    }

                                    foreach (Twitterizer.Coordinate coord in twitterGeo.Coordinates)
                                    {
                                        tweet.GeoInfo.Coordinates.Add(new Domain.Models.Coordinate(tweet) { Longitude = coord.Longitude, Latitude = coord.Latitude });
                                    }
                                }

                                app.Tweets.Add(tweet);

                                foreach (string tag in tweet.Message.GetHashTags())
                                {
                                    if (!IgnoredHashtags.Contains(tag.ToLower()))
                                    {
                                        tweet.AddTag(tag);
                                    }
                                }

                                TweetRepository.Save(tweet);
                            }
                        }

                        TweetRepository.DeleteOldestAfter(MagicValues.MaxTweetsPerApplication, app);
                    }
                }
            }
            else
            {
                base.Interval = TimeSpan.FromMinutes(1.0);
            }
        }

        private Language ProcessLanguage(string isoCode)
        {
            Language language = Languages.SingleOrDefault(l => l.Code.Equals(
                isoCode, StringComparison.InvariantCultureIgnoreCase));

            if (language == null)
            {
                language = new Language();
                language.Code = isoCode.ToLowerInvariant();
                language.Name = null;
                language.IsRecognised = false;

                LanguageRepository.Save(language);

                Languages.Add(language);
            }

            return language;
        }

        protected override void Error(Exception ex, DateTime exceptionTime)
        {
            StringBuilder message = new StringBuilder();

            message.Append(ex.GetType().Name).Append(" on " ).Append(exceptionTime.ToString("d MMM yyyy - h:mm:ss tt")).AppendLine()
                   .AppendLine()
                   .Append(ex.Message).AppendLine()
                   .AppendLine()
                   .Append(ex.StackTrace).AppendLine()
                   .AppendLine()
                   .Append("------------------------------------------------------").AppendLine();

            string appDataFolder = HttpRuntime.AppDomainAppPath + "App_Data\\";

            string file = appDataFolder + "TrawlerErrors.txt";

            File.AppendAllText(file, message.ToString());
        }
    }
}