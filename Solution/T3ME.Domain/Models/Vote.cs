using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Domain.Models
{
    public class Vote : Entity
    {
        protected Vote() { }

        public Vote(Tweet tweet, Tweeter tweeter)
        {
            this.Tweet = tweet;
            this.Tweeter = tweeter;
        }

        public virtual Tweet Tweet { get; protected set; }
        public virtual Tweeter Tweeter { get; protected set; }

        public virtual DateTime VotedDate { get; set; }
    }
}