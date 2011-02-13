using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Domain.Models
{
    public class Tag : Entity
    {
        protected Tag() { }

        public Tag(Tweet tweet)
        {
            this.Tweet = tweet;
        }

        public virtual string Text { get; set; }
        public virtual Tweet Tweet { get; protected set; }
    }
}