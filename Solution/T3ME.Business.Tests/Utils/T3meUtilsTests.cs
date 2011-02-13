using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using T3ME.Business.Utils;

namespace T3ME.Business.Tests.Utils
{
    [TestFixture]
    public class T3meUtilsTests
    {
        #region GetHashTags Tests

        [Test]
        public void GetHashTags_NullString_ReturnEmptyList()
        {
            string message = null;

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual(0, hashTags.Count);
        }

        [Test]
        public void GetHashTags_EmptyString_ReturnEmptyList()
        {
            string message = "";

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual(0, hashTags.Count);
        }

        [Test]
        public void GetHashTags_StringWithNoHashTags_ReturnEmptyList()
        {
            string message = "The cat sat on the mat.";

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual(0, hashTags.Count);
        }

        [Test]
        public void GetHashTags_StringWithOneHashTag_ReturnCollectionWithOneItem()
        {
            string message = "The #cat sat on the mat.";

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual(1, hashTags.Count);
        }

        [Test]
        public void GetHashTags_StringWithOneHashTag_ReturnsThatHashTag()
        {
            string message = "The #cat sat on the mat.";

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual("cat", hashTags[0]);
        }

        [Test]
        public void GetHashTags_StringWithFiveHashTags_ReturnsCollectionWithFiveItems()
        {
            string message = "This #string #contains #at #least #five hashtags.";

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual(5, hashTags.Count);
        }

        [Test]
        public void GetHashTags_StringWithFiveHashTags_ReturnsThoseHashTagsInTheCorrectOrder()
        {
            string message = "This #string #contains #at #least #five hashtags.";

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual("string", hashTags[0]);
            Assert.AreEqual("contains", hashTags[1]);
            Assert.AreEqual("at", hashTags[2]);
            Assert.AreEqual("least", hashTags[3]);
            Assert.AreEqual("five", hashTags[4]);

            Assert.AreEqual(5, hashTags.Count);
        }

        [Test]
        public void GetHashTags_StringWithUpperAndLowerCasing_CasingShouldBeMaintained()
        {
            string message = "The #CaT #SAT on #tHe mat.";

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual("CaT", hashTags[0]);
            Assert.AreEqual("SAT", hashTags[1]);
            Assert.AreEqual("tHe", hashTags[2]);

            Assert.AreEqual(3, hashTags.Count);
        }

        [Test]
        public void GetHashTags_EntireStringOneGiantHashTag_ShouldBeReturned()
        {
            string message = "#ThisIsOneLooooooooongBigHashTag";

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual("ThisIsOneLooooooooongBigHashTag", hashTags[0]);
        }

        [Test]
        public void GetHashTags_LonelyHashSymbol_ShouldBeIgnored()
        {
            string message = "I like cats # and dogs.";

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual(0, hashTags.Count);
        }

        [Test]
        public void GetHashTags_HashTagsStuckTogether_ShouldAllBeReturned()
        {
            string message = "These #hashtags#are#stuck together.";

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual("hashtags", hashTags[0]);
            Assert.AreEqual("are", hashTags[1]);
            Assert.AreEqual("stuck", hashTags[2]);

            Assert.AreEqual(3, hashTags.Count);
        }

        [Test]
        public void GetHashTags_ValidHashTags_CanOnlyContainLettersNumbersAndUnderscores()
        {
            string message = "#The-cat #Sat_On the #mat #99times.";

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual("The", hashTags[0]);
            Assert.AreEqual("Sat_On", hashTags[1]);
            Assert.AreEqual("mat", hashTags[2]);
            Assert.AreEqual("99times", hashTags[3]);

            Assert.AreEqual(4, hashTags.Count);
        }

        [Test]
        public void GetHashTags_HashTagsWithJustNumbers_AreIngored()
        {
            string message = "Here is a hashtag with numbers #12345 which shouldn't be returned.";

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual(0, hashTags.Count);
        }

        [Test]
        public void GetHashTags_SameHashTagMultipleTimes_OnlyReturnedOnce()
        {
            string message = "A bird in the hand is worth more than 2 in the bush #proverb #proverb #proverbs";

            IList<string> hashTags = message.GetHashTags();

            Assert.AreEqual("proverb", hashTags[0]);
            Assert.AreEqual("proverbs", hashTags[1]);

            Assert.AreEqual(2, hashTags.Count);
        }

        [Test]
        public void GetHashTags_SameHashTagMultipleTimesWithDifferentCasing_OnlyReturnedOnce()
        {
            string message = "A bird in the hand is worth more than 2 in the bush #proverb #Proverb #proverbs";

            IList<string> hashTags = message.GetHashTags();

            // We should be returning the first instance, hence the lower-case proverb
            Assert.AreEqual("proverb", hashTags[0]);
            Assert.AreEqual("proverbs", hashTags[1]);

            Assert.AreEqual(2, hashTags.Count);
        }

        #endregion

        #region IsRetweet Tests

        [Test]
        public void IsRetweet_NullString_ReturnsFalse()
        {
            string nullString = null;

            bool isRetweet = nullString.IsRetweet();

            Assert.IsFalse(isRetweet);
        }

        [Test]
        public void IsRetweet_BlankString_ReturnsFalse()
        {
            string blankString = "";

            bool isRetweet = blankString.IsRetweet();

            Assert.IsFalse(isRetweet);
        }

        [Test]
        public void IsRetweet_NormalMessage_ReturnsFalse()
        {
            string message = "Rolling stones gather no moss. #chinese #proverb";

            bool isRetweet = message.IsRetweet();

            Assert.IsFalse(isRetweet);
        }

        [Test]
        public void IsRetweet_MessageWithOfficialTwitterRetweetStyle_ReturnsTrue()
        {
            string message = "RT @Someone: Rolling stones gather no moss. #chinese #proverb";

            bool isRetweet = message.IsRetweet();

            Assert.IsTrue(isRetweet);
        }

        [Test]
        public void IsRetweet_MessageWithOfficialTwitterRetweetStyleInLowercase_ReturnsTrue()
        {
            string message = "rt @Someone: Rolling stones gather no moss. #chinese #proverb";

            bool isRetweet = message.IsRetweet();

            Assert.IsTrue(isRetweet);
        }

        [Test]
        public void IsRetweet_MessageWithAlternativeRetweetStyle_ReturnsTrue()
        {
            string message = "RT: @Someone Rolling stones gather no moss. #chinese #proverb";

            bool isRetweet = message.IsRetweet();

            Assert.IsTrue(isRetweet);
        }

        [Test]
        public void IsRetweet_MessageWithOfficialTwitterRetweetStyleSansColon_ReturnsTrue()
        {
            string message = "RT @Someone Rolling stones gather no moss. #chinese #proverb";

            bool isRetweet = message.IsRetweet();

            Assert.IsTrue(isRetweet);
        }

        [Test]
        public void IsRetweet_MessageWithRetweetStyleAnywhereInString_ReturnsTrue()
        {
            string message = "Rolling stones gather no moss RT @Someone: #chinese #proverb";

            bool isRetweet = message.IsRetweet();

            Assert.IsTrue(isRetweet);
        }

        [Test]
        public void IsRetweet_MessageWithRetweetStyleAnywhereInStringSansColon_ReturnsTrue()
        {
            string message = "Rolling stones gather no moss RT @Someone #chinese #proverb";

            bool isRetweet = message.IsRetweet();

            Assert.IsTrue(isRetweet);
        }

        [Test]
        public void IsRetweet_MessageWithRTSomewhereButNoProceedingUsername_ReturnsFalse()
        {
            string message = "RT Rolling stones gather no moss. #chinese #proverb";

            bool isRetweet = message.IsRetweet();

            Assert.IsFalse(isRetweet);
        }

        #endregion
    }
}