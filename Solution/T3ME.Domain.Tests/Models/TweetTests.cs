using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using T3ME.Domain.Models;
using T3ME.Domain.Models.Enums;
using Iesi.Collections.Generic;
using System.Reflection;

namespace T3ME.Domain.Tests.Models
{
    [TestFixture]
    public class TweetTests
    {
        private Tweet CreateTweet()
        {
            Application app = new Application();
            Tweeter tweeter = new Tweeter();

            return new Tweet(app, tweeter);
        }

        [Test]
        public void Geo_NewTweet_ReturnsNullGeo()
        {
            Tweet tweet = CreateTweet();
            Assert.IsNull(tweet.GeoInfo);
        }

        [Test]
        public void Geo_TweetWithGeo_ShouldReturnGeoInfo()
        {
            // Arrange
            Tweet tweet = CreateTweet();
            tweet.GeoInfo = new Tweet.Geo();

            // Act
            Tweet.Geo geoInfo = tweet.GeoInfo;

            // Assert
            Assert.IsNotNull(geoInfo);
        }

        [Test]
        public void Geo_TweetWithGeoShape_ShouldReturnCorrectShape()
        {
            // Arrange
            Tweet tweet = CreateTweet();
            tweet.GeoInfo = new Tweet.Geo() { Shape = GeoShape.Polygon };

            // Act
            Tweet.Geo geoInfo = tweet.GeoInfo;

            // Assert
            Assert.AreEqual(GeoShape.Polygon, geoInfo.Shape);
        }

        [Test]
        public void Geo_TweetWithGeoCoords_ShouldReturnCorrectNumberOfCoords()
        {
            // Arrange
            Tweet tweet = CreateTweet();
            tweet.GeoInfo = new Tweet.Geo();
            tweet.GeoInfo.Coordinates.Add(new Coordinate(tweet));
            tweet.GeoInfo.Coordinates.Add(new Coordinate(tweet));
            tweet.GeoInfo.Coordinates.Add(new Coordinate(tweet));

            // Act
            Tweet.Geo geoInfo = tweet.GeoInfo;
            ISet<Coordinate> coords = geoInfo.Coordinates;

            // Assert
            Assert.AreEqual(3, coords.Count);
        }

        [Test]
        public void Geo_TweetWithGeoCoords_ShouldReturnCorrectGeoCoordInfo()
        {
            // Arrange
            Tweet tweet = CreateTweet();
            tweet.GeoInfo = new Tweet.Geo();
            tweet.GeoInfo.Coordinates.Add(new Coordinate(tweet) { Latitude = 2.0, Longitude = 6.0 });
            tweet.GeoInfo.Coordinates.Add(new Coordinate(tweet) { Latitude = -1.0, Longitude = 1.0 });
            tweet.GeoInfo.Coordinates.Add(new Coordinate(tweet) { Latitude = 4.0, Longitude = -8.0 });

            // Act
            Tweet.Geo geoInfo = tweet.GeoInfo;
            ISet<Coordinate> coords = geoInfo.Coordinates;

            // Assert
            Assert.AreEqual(6.0, coords.ElementAt(0).Longitude);
            Assert.AreEqual(-1.0, coords.ElementAt(1).Latitude);
            Assert.AreEqual(4.0, coords.ElementAt(2).Latitude);
        }

        [Test]
        public void Geo_TweetWithGeo_AssignmentByBuildingEntireObject()
        {
            // Arrange
            Tweet tweet = CreateTweet();

            Tweet.Geo geo = new Tweet.Geo();
            geo.Shape = GeoShape.Point;
            geo.Coordinates.Add(new Coordinate(tweet) { Latitude = 10.0, Longitude = 5.0 });

            tweet.GeoInfo = geo;

            // Act
            Tweet.Geo actualGeo = tweet.GeoInfo;

            // Assert
            Assert.AreEqual(GeoShape.Point, actualGeo.Shape, "GeoShape is incorrect");
            Assert.AreEqual(10.0, actualGeo.Coordinates.ElementAt(0).Latitude, "Latitude is incorrect");
            Assert.AreEqual(5.0, actualGeo.Coordinates.ElementAt(0).Longitude, "Longitude is incorrect");
        }

        [Test]
        public void Geo_TweetWithGeo_AssignmentByPropertyAccessor()
        {
            // Arrange
            Tweet tweet = CreateTweet();

            tweet.GeoInfo = new Tweet.Geo();
            tweet.GeoInfo.Shape = GeoShape.CircleByCenterPoint;
            tweet.GeoInfo.Coordinates.Add(new Coordinate(tweet) { Latitude = 10.0, Longitude = 5.0 });

            // Act
            GeoShape shape = tweet.GeoInfo.Shape;
            Coordinate coord = tweet.GeoInfo.Coordinates.ElementAt(0);

            // Assert
            Assert.AreEqual(GeoShape.CircleByCenterPoint, shape, "GeoShape is incorrect");
            Assert.AreEqual(10.0, coord.Latitude, "Latitude is incorrect");
            Assert.AreEqual(5.0, coord.Longitude, "Longitude is incorrect");
        }

        [Test]
        public void Geo_TweetWithGeo_AssigningNullToGeoSetsToNull()
        {
            // Arrange
            Tweet tweet = CreateTweet();

            tweet.GeoInfo = new Tweet.Geo();
            tweet.GeoInfo.Shape = GeoShape.CircleByCenterPoint;
            tweet.GeoInfo.Coordinates.Add(new Coordinate(tweet) { Latitude = 10.0, Longitude = 5.0 });

            // Act
            tweet.GeoInfo = null;

            // Assert
            Assert.IsNull(tweet.GeoInfo);
        }

        [Test]
        public void Geo_TweetFromDataStoreWithShape_ShouldReturnGeoWithShape()
        {
            // Arrange
            Tweet tweet = CreateTweet();

            tweet.GetType().GetProperty("GeoShapeType", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(tweet, GeoShape.LineString, null);

            // Act
            Tweet.Geo geoInfo = tweet.GeoInfo;

            // Assert
            Assert.IsNotNull(geoInfo);
            Assert.AreEqual(GeoShape.LineString, geoInfo.Shape);
        }

        [Test]
        public void Geo_TweetFromDataStoreWithCoordinates_ShouldReturnGeoWithCoords()
        {
            // Arrange
            Tweet tweet = CreateTweet();

            tweet.GetType().GetProperty("GeoShapeType", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(tweet, GeoShape.Polygon, null);

            ISet<Coordinate> coords = new HashedSet<Coordinate>();
            coords.Add(new Coordinate(tweet) { Latitude = 1.0, Longitude = 2.0 });
            coords.Add(new Coordinate(tweet) { Latitude = 3.0, Longitude = 4.0 });
            tweet.GetType().GetProperty("Coordinates", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(tweet, coords, null);

            // Act
            Tweet.Geo geoInfo = tweet.GeoInfo;

            // Assert
            Assert.IsNotNull(geoInfo);
            Assert.AreEqual(1.0, geoInfo.Coordinates.ElementAt(0).Latitude);
            Assert.AreEqual(2.0, geoInfo.Coordinates.ElementAt(0).Longitude);
            Assert.AreEqual(3.0, geoInfo.Coordinates.ElementAt(1).Latitude);
            Assert.AreEqual(4.0, geoInfo.Coordinates.ElementAt(1).Longitude);
        }

        [Test]
        public void Geo_TweetWithGeoShape_HiddenPropertiesShouldBetSet()
        {
            // Arrange
            Tweet tweet = CreateTweet();
            tweet.GeoInfo = new Tweet.Geo() { Shape = GeoShape.Polygon };

            // Act
            GeoShape? actualShape = (GeoShape?)tweet.GetType()
                .GetProperty("GeoShapeType", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(tweet, null);

            // Assert
            Assert.IsNotNull(actualShape);
            Assert.AreEqual(GeoShape.Polygon, actualShape.Value);
        }

        [Test]
        public void Geo_TweetWithGeoCoords_HiddenPropertiesShouldBetSet()
        {
            // Arrange
            Tweet tweet = CreateTweet();
            tweet.GeoInfo = new Tweet.Geo();
            tweet.GeoInfo.Coordinates.Add(new Coordinate(tweet) { Latitude = 1.0, Longitude = 2.0 });

            // Act
            Coordinate actualCoord = ((ISet<Coordinate>)tweet.GetType()
                .GetProperty("Coordinates", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(tweet, null)).ElementAt(0);

            // Assert
            Assert.AreEqual(1.0, actualCoord.Latitude);
            Assert.AreEqual(2.0, actualCoord.Longitude);
        }
    }
}