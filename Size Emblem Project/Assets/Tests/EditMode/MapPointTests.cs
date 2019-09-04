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
            var point1 = new MapPoint(10, 20, 30, 40);
            Assert.AreEqual(10, point1.X);
            Assert.AreEqual(20, point1.Y);
            Assert.AreEqual(30, point1.Width);
            Assert.AreEqual(40, point1.Height);

            // Create a second instance with different constructor parameters to make sure the class isn't doing weird lifestyle/static stuff
            var point2 = new MapPoint(11, 22, 33, 44);
            Assert.AreEqual(11, point2.X);
            Assert.AreEqual(22, point2.Y);
            Assert.AreEqual(33, point2.Width);
            Assert.AreEqual(44, point2.Height);

            Assert.AreNotEqual(point1.X, point2.X);
            Assert.AreNotEqual(point1.Y, point2.Y);
            Assert.AreNotEqual(point1.Width, point2.Width);
            Assert.AreNotEqual(point1.Height, point2.Height);
        }


        [Test(Description ="Test collision of 1x1 map points")]
        public void MapPointCollisionTest1x1()
        {
            var point1 = new MapPoint(100, 100, 1, 1);
            var point2 = new MapPoint(100, 100, 1, 1); // Same location map point, will collide
            var pointR = new MapPoint(101, 100, 1, 1); // Map point one to the right
            var pointL = new MapPoint(99,  100, 1, 1); // Map point one to the left
            var pointT = new MapPoint(100, 101, 1, 1); // Map point one tile above
            var pointB = new MapPoint(100, 99, 1, 1); // Map point one tile above

            // Same X,Y and 1x1 dimension test
            Assert.IsTrue(point1.CollidesWith(point2), "Point1 does NOT collide with Point2");
            // Test for symmetry
            Assert.IsTrue(point2.CollidesWith(point1), "Point2 does NOT collide with Point1");

            // Test a point that is off by 1 for 1x1 area points don't collide
            Assert.IsFalse(point1.CollidesWith(pointR), "Point1 DOES collide with PointR");
            Assert.IsFalse(point2.CollidesWith(pointR), "Point2 DOES collide with PointR");
            // Symmetry test
            Assert.IsFalse(pointR.CollidesWith(point1), "PointR DOES collide with Point1");
            Assert.IsFalse(pointR.CollidesWith(point2), "PointR DOES collide with Point2");

            // More map point tests that are off by one in other directions
            Assert.IsFalse(point1.CollidesWith(pointL), "Point1 DOES collide with PointL");
            Assert.IsFalse(pointL.CollidesWith(point1), "PointL DOES collide with Point1");

            Assert.IsFalse(point1.CollidesWith(pointT), "Point1 DOES collide with PointT");
            Assert.IsFalse(pointT.CollidesWith(point1), "PointT DOES collide with Point1");

            Assert.IsFalse(point1.CollidesWith(pointB), "Point1 DOES collide with PointB");
            Assert.IsFalse(pointB.CollidesWith(point1), "PointB DOES collide with Point1");
        }


    }
}
