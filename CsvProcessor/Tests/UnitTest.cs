using System.Collections.Generic;
using CsvProcessor.Helpers;
using NUnit.Framework;

namespace CSV_Processor.Tests
{
    [TestFixture]
    public class PercentageAboveOrBelowTests
    {
        [TestCase]
        public void WhenValueWithin20Percent()
        {
            // Arrange & Act
            Assert.IsFalse(Maths.PercentageAboveOrBelow(100.0, 100.0, 0.2));
            Assert.IsFalse(Maths.PercentageAboveOrBelow(100.0, 80.0, 0.2));
        }
        
        [TestCase]
        public void WhenValueOutside20Percent()
        {          
            // Arrange & Act
            Assert.IsTrue(Maths.PercentageAboveOrBelow(100, 79.9, 0.2));
            Assert.IsTrue(Maths.PercentageAboveOrBelow(100, 65.2, 0.2));
        }
    }

    public class MedianTests
    {
        [TestCase]
        public void WhenEvenNumberOfValues()
        {
            // Arrange
            var dataList = new List<double>(){4,4,4,4,3,2,2,1};

            // Act
            Assert.AreEqual(3.5, Maths.Median(dataList));
        }

        [TestCase]
        public void WhenOldNumberOfValues() 
        {
            // Arrange
            var dataList = new List<double>() { 4, 5, 6, 4, 3, 2, 2,10,20 };
            // Act
            Assert.AreEqual(4.0, Maths.Median(dataList));
        }
    }
}
