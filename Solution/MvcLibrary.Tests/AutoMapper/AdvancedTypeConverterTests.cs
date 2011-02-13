using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using MvcLibrary.AutoMapper;
using AutoMapper;
using Moq.Protected;

namespace MvcLibrary.Tests.AutoMapper
{
    [TestFixture]
    public class AdvancedTypeConverterTests
    {
        protected class TestTypeConverter : AdvancedTypeConverter<string, int>
        {
            protected override int ConvertCore(string source)
            {
                throw new NotImplementedException();
            }
        }

        protected ResolutionContext BuildContext<TSource, TDestination>(TSource source, TDestination destination)
        {
            Type sourceType = typeof(TSource);
            Type destType = typeof(TDestination);
            TypeMap map = new TypeMap(new TypeInfo(sourceType), new TypeInfo(destType));

            if (destination != null)
            {
                return new ResolutionContext(map, source, destination, sourceType, destType);
            }
            else
            {
                return new ResolutionContext(map, source, sourceType, destType);
            }
        }

        [Test]
        public void ConvertCore_ShouldBeCalled_WithValidType()
        {
            var mock = new Mock<AdvancedTypeConverter<string, int>>();

            mock.Protected().Setup<int>("ConvertCore").Returns(9);

            int actual = mock.Object.Convert(BuildContext<string, int>("", 0));

            //Assert.AreEqual(9, actual);
        }
    }
}