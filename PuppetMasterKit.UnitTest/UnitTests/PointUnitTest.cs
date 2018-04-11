
using System;
using NUnit.Framework;
using PuppetMasterKit.Graphics;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.UnitTest
{
    [TestFixture]
    public class PointUnitTest
    {
        [Test]
        public void SortByDistance()
        {
            Point reference = new Point(0, 0);
            Point[] points = { new Point(10, 20), 
                new Point(2,3), 
                new Point(1,1) };
            var result = reference.SortByDistance(points);

            Assert.True(result[0] == new Point(1, 1));
            Assert.True(result[1] == new Point(2, 3));
            Assert.True(result[2] == new Point(10, 20));
        }

        [Test]
        public void Equality(){

            Assert.False(new Point(0, 0.01f) == new Point(0, 0.02f));
            Assert.True(new Point(0,0.012f) == new Point(0,0.013f));
        }
    }
}
