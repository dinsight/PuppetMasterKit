
using System;
using NUnit.Framework;
using PuppetMasterKit.Graphics;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.UnitTest
{
    [TestFixture]
    public class VectorUnitTest
    {
        [Test]
        public void Magnitude()
        {
            var vector = new Vector(10,10);
            Assert.AreEqual(vector.Magnitude(), Math.Sqrt(200));
        }

        [Test]
        public void VectorOperators()
        {
            var vector = new Vector(10, 10);
            var res = vector + new Point(0, 0);
            var res2 = new Point(0, 0) + vector;
        }
    }
}
