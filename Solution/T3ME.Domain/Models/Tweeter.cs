using System;
using System.Linq;
using System.Text;
using Iesi.Collections.Generic;

namespace T3ME.Domain.Models
{
    public class Tweeter : Entity
    {
        public Tweeter()
        {
            this.Tweets = new HashedSet<Tweet>();
            this.PersistentLogins = new HashedSet<PersistentLogin>();
        }

        public virtual long TwitterId { get; set; }
        public virtual string Username { get; set; }
        public virtual string FullName { get; set; }
        public virtual string ImageUrl { get; set; }

        public virtual string BackgroundImageUrl { get; set; }
        public virtual bool IsBackgroundImageTiled { get; set; }

        public virtual string Bio { get; set; }
        public virtual string Website { get; set; }
        public virtual string Location { get; set; }

        public virtual int NumberOfTweets { get; set; }
        public virtual int NumberOfFollowers { get; set; }
        public virtual int NumberFollowing { get; set; }
        public virtual int NumberListed { get; set; }

        public virtual int TimeZoneOffset { get; set; }

        public virtual bool IsProtected { get; set; }

        public virtual Language Language { get; set; }
        public virtual Language DefaultLanguage { get; set; }

        public virtual ISet<Tweet> Tweets { get; protected set; }

        public virtual ISet<PersistentLogin> PersistentLogins { get; protected set; }
    }
}