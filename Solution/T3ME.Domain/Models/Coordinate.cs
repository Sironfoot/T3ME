using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Domain.Models
{
    public class Coordinate : Entity
    {
        protected Coordinate() { }

        public Coordinate(Tweet tweet)
        {
            this.Tweet = tweet;
        }

        public virtual double Longitude { get; set; }
        public virtual double Latitude { get; set; }

        public virtual Tweet Tweet { get; set; }
    }
}