using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using T3ME.Business.Controllers;
using T3ME.Domain.Repositories;
using T3ME.Domain.Models;
using Twitterizer;
using T3ME.Business.Utils;

namespace T3ME.Web.Controllers
{
    public class VoteController : MasterController
    {
        public VoteController(IApplicationRepository appRepository, ITweetRepository tweetRepository,
            ITweeterRepository tweeterRepository, ILanguageRepository languageRepository)
            : base(appRepository, tweetRepository, tweeterRepository, languageRepository) { }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public object RegisterVote(long tweetId)
        {
            if (Tweeter != null)
            {
                Tweet tweet = TweetRepository.FromTwitterId(tweetId, App);

                // TODO: Optimise tweet.Votes.Any
                if (tweet != null && !tweet.Votes.Any(v => v.Tweeter == Tweeter))
                {
                    tweet.CastVote(Tweeter);
                    TweetRepository.Save(tweet);

                    bool requiresTweeting = tweet.Votes.Count == MagicValues.NumVotesForRetweeting;
                    bool shouldFollowTweeter = TweeterRepository.TotalTweetsVotedForTweeter(tweet.Tweeter, App)
                        == MagicValues.NumTweetsVotedForBeforeFollowing;

                    OAuthTokens token = null;

                    if(requiresTweeting || shouldFollowTweeter)
                    {
                        token = new OAuthTokens();
                        token.ConsumerKey = MagicValues.OAuthConsumerKey;
                        token.ConsumerSecret = MagicValues.OAuthConsumerSecret;
                        token.AccessToken = App.Account.AccessToken;
                        token.AccessTokenSecret = App.Account.TokenSecret;
                    }

                    if (requiresTweeting)
                    {
                        string updateText = tweet.Message + " (via @" + tweet.Tweeter.Username + ")";

                        if (updateText.Length > 140)
                        {
                            TwitterStatus.Retweet(token, (decimal)tweet.TwitterId);
                        }
                        else
                        {
                            TwitterStatus.Update(token, updateText);
                        }
                    }

                    if (shouldFollowTweeter)
                    {
                        TwitterFriendship.Create(token, tweet.Tweeter.TwitterId);
                    }
                }
            }
            else
            {
                if (Request.IsAjaxRequest())
                {
                    return "signin";
                }
                else
                {
                    return RedirectToAction("Signin", "OAuth");
                }
            }

            if (!Request.IsAjaxRequest())
            {
                if (Request.UrlReferrer != null && Request.UrlReferrer.Host == App.Url)
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }

                return RedirectToAction("Latest", "Display");
            }
            else
            {
                return "ok";
            }
        }
    }
}