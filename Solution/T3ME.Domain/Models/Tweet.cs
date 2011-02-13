using System;
using System.Linq;
using System.Text;
using T3ME.Domain.Models.Components;
using Iesi.Collections.Generic;
using T3ME.Domain.Models.Enums;

namespace T3ME.Domain.Models
{
    public class Tweet : Entity
    {
        protected Tweet()
        {
            Votes = new HashedSet<Vote>();
            Tags = new HashedSet<Tag>();
            Coordinates = new HashedSet<Coordinate>();
        }

        public Tweet(Application app, Tweeter tweeter) : this()
        {
            this.App = app;
            this.Tweeter = tweeter;
        }

        public virtual long TwitterId { get; set; }
        public virtual string Message { get; set; }
        public virtual DateTime PostedDate { get; set; }
        public virtual Device Device { get; set; }
        public virtual DateTime? FirstVoteCast { get; set; }

        public virtual Application App { get; protected set; }
        public virtual Tweeter Tweeter { get; protected set; }
        public virtual Language Language { get; set; }

        public virtual string Location { get; set; }
        protected virtual GeoShape? GeoShapeType { get; set; }
        protected virtual ISet<Coordinate> Coordinates { get; set; }

        public virtual Geo GeoInfo
        {
            get
            {
                if (GeoShapeType != null)
                {
                    return new Geo(this);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    if (value.Tweet == null)
                    {
                        GeoShapeType = value.Shape;
                        Coordinates.Clear();

                        Coordinates.AddAll(value.Coordinates);
                    }
                }
                else
                {
                    GeoShapeType = null;
                    Coordinates.Clear();
                }
            }
        }

        public virtual ISet<Vote> Votes { get; protected set; }
        public virtual ISet<Tag> Tags { get; protected set; }

        public virtual void CastVote(Tweeter tweeter)
        {
            Vote vote = new Vote(this, tweeter);
            vote.VotedDate = DateTime.UtcNow;

            bool wasAdded = Votes.Add(vote);

            if (wasAdded)
            {
                if (FirstVoteCast == null)
                {
                    FirstVoteCast = DateTime.UtcNow;
                }
            }
        }

        public virtual void AddTag(string text)
        {
            Tag tag = new Tag(this);
            tag.Text = text;

            this.Tags.Add(tag);
        }

        public class Geo
        {
            internal Tweet Tweet = null;

            internal Geo(Tweet tweet)
            {
                Tweet = tweet;
                _shape = tweet.GeoShapeType.GetValueOrDefault(GeoShape.Point);
                Coordinates = tweet.Coordinates;
            }

            public Geo()
            {
                Coordinates = new HashedSet<Coordinate>();
            }

            private GeoShape _shape;
            public GeoShape Shape
            {
                get { return _shape; }
                set
                {
                    _shape = value;

                    if (Tweet != null)
                    {
                        Tweet.GeoShapeType = _shape;
                    }
                }
            }

            public ISet<Coordinate> Coordinates { get; protected set; }
        }
    }
}