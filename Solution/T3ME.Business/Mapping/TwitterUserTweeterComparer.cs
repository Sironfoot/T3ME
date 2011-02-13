using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twitterizer;
using T3ME.Domain.Models;

namespace T3ME.Business.Mapping
{
    public class TwitterUserTweeterComparer
    {
        public static bool AreSame(TwitterUser user, Tweeter tweeter)
        {
            if (user == null || tweeter == null) return false;

            if (tweeter.TwitterId != (long)user.Id ||
                (tweeter.Language != null && !tweeter.Language.Code.Equals(user.Language, StringComparison.InvariantCultureIgnoreCase)) ||
                tweeter.Username != user.ScreenName ||
                tweeter.FullName != user.Name ||
                tweeter.ImageUrl != user.ProfileImageLocation ||
                tweeter.BackgroundImageUrl != user.ProfileBackgroundImageLocation ||
                tweeter.IsBackgroundImageTiled != user.IsProfileBackgroundTiled ||
                tweeter.Bio != user.Description ||
                tweeter.Website != user.Website ||
                tweeter.Location != user.Location ||
                tweeter.NumberOfTweets != user.NumberOfStatuses ||
                tweeter.NumberOfFollowers != user.NumberOfFollowers ||
                tweeter.NumberFollowing != user.NumberOfFriends ||
                tweeter.NumberListed != user.NumberOfFavorites ||
                tweeter.TimeZoneOffset != (int)user.TimeZoneOffset.GetValueOrDefault(0.0) ||
                tweeter.IsProtected != user.IsProtected)
            {
                return false;
            }

            return true;
        }
    }
}