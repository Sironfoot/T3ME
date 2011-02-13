using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Domain.Models.Components
{
    public class TwitterAccount
    {
        protected TwitterAccount() { }

        public TwitterAccount(long userId, string screenName, string accessToken, string tokenSecret)
        {
            this.UserId = userId;
            this.ScreenName = screenName;
            this.AccessToken = accessToken;
            this.TokenSecret = tokenSecret;
        }

        public long UserId { get; protected set; }
        public string ScreenName { get; protected set; }
        public string AccessToken { get; protected set; }
        public string TokenSecret { get; protected set; }
    }
}