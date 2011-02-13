using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace T3ME.Business.Utils
{
    public static class TwitterUrls
    {
        public const string Domain = "http://twitter.com";

        public static string UserProfile(string username)
        {
            return Domain + "/" + HttpUtility.UrlEncode(username);
        }

        public static string Reply(string username, long twitterMessageId)
        {
            return Domain + "/?status=" + HttpUtility.UrlEncode("@" + username) + "%20" +
                "&in_reply_to_status_id=" + twitterMessageId + "&in_reply_to=" + HttpUtility.UrlEncode(username);
        }

        public static string Retweet(string username, string message)
        {
            string retweetMessage = "RT @" + username + ": " + message;
            return Domain + "/?status=" + HttpUtility.UrlEncode(retweetMessage).Replace("+", "%20"); // God dam twitter doesn't understand '+' means space
        }

        public static string PostMessage(string message)
        {
            return Domain + "/?status=" + HttpUtility.UrlEncode(message).Replace("+", "%20");
        }
    }
}