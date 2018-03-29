using System;
using System.Collections.Generic;
using System.Linq;
using PuppetMasterKit.Graphics;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.Graphics.Geometry
{
    public class Point
    {
        public static Point Zero = new Point(0, 0);
        
        public float X{get; set;}

        public float Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Geometry.Point"/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Point(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Scalar multiplication
        /// </summary>
        /// <param name="point">The <see cref="PuppetMasterKit.Geometry.Point"/> to multiply.</param>
        /// <param name="scalar">The <see cref="float"/> to multiply.</param>
        /// <returns>The <see cref="T:PuppetMasterKit.Geometry.Point"/> that is the <c>point</c> * <c>scalar</c>.</returns>
        public static Point operator *(Point point, float scalar)
        {
            return new Point(point.X * scalar, point.Y * scalar);
        }

        /// <summary>
        /// Addition
        /// </summary>
        /// <param name="lhs">The first <see cref="PuppetMasterKit.Geometry.Point"/> to add.</param>
        /// <param name="rhs">The second <see cref="PuppetMasterKit.Geometry.Point"/> to add.</param>
        /// <returns>The <see cref="T:PuppetMasterKit.Geometry.Point"/> that is the sum of the values of <c>lhs</c> and <c>rhs</c>.</returns>
        public static Point operator +(Point lhs, Point rhs)
        {
            return new Point(lhs.X + rhs.X, lhs.Y + lhs.Y);
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="lhs">The first <see cref="PuppetMasterKit.Geometry.Point"/> to compare.</param>
        /// <param name="rhs">The second <see cref="PuppetMasterKit.Geometry.Point"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Point lhs, Point rhs)
        {
            if (Object.ReferenceEquals(lhs, null) 
                && Object.ReferenceEquals(rhs, null))
                return true;

            if (!Object.ReferenceEquals(lhs, null) 
                && !Object.ReferenceEquals(rhs, null)) {
                return Math.Abs(lhs.X - rhs.X) < Float.EPSILON
                       && Math.Abs(lhs.Y - rhs.Y) < Float.EPSILON;
            }
            return false;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:PuppetMasterKit.Geometry.Point"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="T:PuppetMasterKit.Geometry.Point"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Point) {
                return this == (Point)obj;
            }
            return false;
		}

        /// <summary>
        /// Hash code
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            checked
            {
                var val = 31;
                val = val = (val * 17) & +X.GetHashCode();
                val = val = (val * 17) & +Y.GetHashCode();
                return val;
            }
		}

		/// <summary>
		/// Difference between two point 
		/// </summary>
		/// <param name="lhs">The <see cref="PuppetMasterKit.Geometry.Point"/> to subtract from (the minuend).</param>
		/// <param name="rhs">The <see cref="PuppetMasterKit.Geometry.Point"/> to subtract (the subtrahend).</param>
		/// <returns>The <see cref="T:PuppetMasterKit.Geometry.Vector"/> that is the <c>lhs</c> minus <c>rhs</c>.</returns>
        public static Vector operator -(Point lhs, Point rhs)
        {
            return new Vector(lhs.X - rhs.X, lhs.Y - lhs.Y);
        }

        /// <summary>
        /// Inquality operator
        /// </summary>
        /// <param name="lhs">The first <see cref="PuppetMasterKit.Geometry.Point"/> to compare.</param>
        /// <param name="rhs">The second <see cref="PuppetMasterKit.Geometry.Point"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Point lhs, Point rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Distance between two points
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="point1">Point1.</param>
        /// <param name="point2">Point2.</param>
        public static float Distance(Point point1, Point point2) 
        {
            var dxsq = ((point1.X - point2.X) * (point1.X - point2.X));
            var dysq = ((point1.Y - point2.Y) * (point1.Y - point2.Y));
            return (float)Math.Sqrt(dxsq + dysq);
        }

        /// <summary>
        /// Gets the closest point to this point from the array
        /// </summary>
        /// <returns>The closest.</returns>
        /// <param name="points">Points.</param>
        public Point Closest(Point[] points)
        {
            float dist = float.MaxValue;
            Point res = null;

            foreach (var item in points) {
                var d = Point.Distance(this, item);
                if (d<dist) {
                    dist = d;
                    res = item;
                }
            }
            return res;
        }

        /// <summary>
        /// Sorts points by closeness to this point
        /// </summary>
        /// <returns>The by distance.</returns>
        /// <param name="points">Points.</param>
        public Point[] SortByDistance(Point[] points) 
        {
            List<Point> list = new List<Point>(points);
            list.Sort((x, y) => (int)(Distance(this, x) - Distance(x, y)));
            return list.ToArray();
        }
    }
}
