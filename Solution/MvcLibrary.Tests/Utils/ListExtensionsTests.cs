using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MvcLibrary.Utils;

namespace MvcLibrary.Tests.Utils
{
    [TestFixture]
    public class ListExtensionsTests
    {
        [Test]
        public void TakeColumn_NullList_ReturnsEmptyList()
        {
            IList<int> items = null;

            IList<int> actualItems = items.TakeColumn(1, 3);

            Assert.AreEqual(0, actualItems.Count);
        }

        [Test]
        public void TakeColumn_ListSmallerThanNumberOfColumns_ShouldReduceColumnCountToNumberOfItems()
        {
            // Arrange - collections of 5 items
            IList<int> items = new List<int>() { 1, 2, 3, 4, 5 };

            // Act - get the 10th column even though there's only 5 items
            IList<int> actualItems = items.TakeColumn(10, 10);

            // Assert - internally method should just assume last column (eg. 5)
            Assert.AreEqual(5, actualItems[0]);
        }

        [Test]
        public void TakeColumn_Items3Column3of3_ReturnsThirdItem()
        {
            // Arrange items
            IList<int> items = new List<int>() { 1, 2, 3 };

            // Act
            IList<int> actualItems = items.TakeColumn(3, 3);

            // Assert
            Assert.AreEqual(3, actualItems[0]);
        }

        [Test]
        public void TakeColumn_Items30AnyColumnOf3_Returns10Items()
        {
            // Arrange - collection containing 30 items
            List<int> items = new List<int>();
            for (int i = 1; i <= 30; i++)
            {
                items.Add(i);
            }

            // Act
            IList<int> actualItems = items.TakeColumn(1, 3);

            // Assert - collection has 10 items
            Assert.AreEqual(10, actualItems.Count);
        }

        [Test]
        public void TakeColumn_Items29Column3of3_ReturnsOnly9Items()
        {
            // Arrange - collection containing 29 items
            List<int> items = new List<int>();
            for (int i = 1; i <= 29; i++)
            {
                items.Add(i);
            }

            // Act
            IList<int> actualItems = items.TakeColumn(3, 3);

            // Assert - last column should have only 9 items
            Assert.AreEqual(9, actualItems.Count);
        }
    }
}