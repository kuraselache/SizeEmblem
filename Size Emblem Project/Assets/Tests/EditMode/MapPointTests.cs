using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SizeEmblem.Scripts.Containers;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MapPointTests
    {
        [Test]
        public void MapPointConstructorTest()
        {
            // Make sure the constructor parameters are assigned and in the correct order
            var point = new MapPoint(10, 20, 30, 40);
            Assert.AreEqual(10, point.X);
            Assert.AreEqual(20, point.Y);
            Assert.AreEqual(30, point.Width);
            Assert.AreEqual(40, point.Height);
        }


        [Test]
        public void MapPointCollisionTest()
        {
            var point1 = new MapPoint(100, 100, 1, 1);
            var point2 = new MapPoint(100, 100, 1, 1);
            var point3 = new MapPoint(101, 100, 1, 1);

            // Same X,Y and 1x1 dimension test
            Assert.IsTrue(point1.CollidesWith(point2));
            // Test for symmetry
            Assert.IsTrue(point2.CollidesWith(point1));

            // Test a point that is off by 1 for 1x1 area points don't collide
            Assert.IsFalse(point1.CollidesWith(point3));
            Assert.IsFalse(point2.CollidesWith(point3));
            // Symmetry test
            Assert.IsFalse(point3.CollidesWith(point1));
            Assert.IsFalse(point3.CollidesWith(point2));
        }
    }
}
