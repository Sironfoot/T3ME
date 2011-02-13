using System;
using Iesi.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Domain.Models.Components;

namespace T3ME.Domain.Models
{
    public class Application : Entity
    {
        public Application()
        {
            CreatedDate = DateTime.Now;

            HashTags = new HashedSet<string>();
            NotFoundTweets = new HashedSet<string>();
            Tweets = new HashedSet<Tweet>();
            Stylesheets = new HashedSet<Stylesheet>();
        }

        public virtual string Title { get; set; }
        public virtual string SubTitle { get; set; }
        public virtual string Blurb { get; set; }
        public virtual string Url { get; set; }
        public virtual Noun Noun { get; set; }
        public virtual string ConsumerKey { get; set; }
        public virtual string ConsumerSecret { get; set; }
        public virtual string GoogleAnalyticsCode { get; set; }

        public virtual TwitterAccount Account { get; set; }

        public virtual DateTime CreatedDate { get; set; }

        public virtual ISet<string> HashTags { get; protected set; }
        public virtual ISet<string> NotFoundTweets { get; protected set; }

        public virtual ISet<Tweet> Tweets { get; protected set; }
        public virtual ISet<Stylesheet> Stylesheets { get; protected set; }
    }
}