using System;
using System.Collections.Generic;
using System.Linq;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.Geometry
{
    public class Segment
    {

        public Point Start { get; private set; }

        public Point End { get; private set; }

        public float? Slope { get; private set; }

        public float? Intercept { get; private set; }

        public int DecimalPlaces { get; private set; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:PuppetMasterKit.Geometry.Segment"/> class.
        /// </summary>
        public Segment(Point start, Point end)
        {
            DecimalPlaces = Float.Decimals;
            this.Start = start;
            this.End = end;
            this.Slope = CalcSlope();
            this.Intercept = CalcIntercept();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="points">Points.</param>
        public Segment(Point[] points)
        {
            DecimalPlaces = Float.Decimals;

            if (points.Length == 1) {
                this.Start = points[0];
                this.End = points[0];
            } else {
                this.Start = points[0];
                this.End = points[1];
            }

            this.Slope = CalcSlope();
            this.Intercept = CalcIntercept();
        }

        /// <summary>
        /// Calculates the slope.
        /// </summary>
        /// <returns>The slope.</returns>
        private float? CalcSlope()
        {
            if (Math.Abs(Start.X - End.X) > Float.EPSILON)
            {
                return (End.Y - Start.Y) / (End.X - Start.X);
            }
            return null;
        }

        /// <summary>
        /// Calculates the intercept.
        /// </summary>
        /// <returns>The intercept.</returns>
        private float? CalcIntercept()
        {
            var slope = CalcSlope();

            if (slope != null)
            {
                return Start.Y - slope * Start.X;
            }
            return null;
        }

        /// <summary>
        /// Check if point is inside the line segment
        /// </summary>
        /// <returns><c>true</c>, if point is inside the line segment, 
        /// <c>false</c> otherwise.</returns>
        /// <param name="point">Point.</param>
        public bool IsInside(Point point)
        {
            if (point == null)
                return false;

            if (Math.Abs(Start.X - End.X) < Float.EPSILON
                && Math.Abs(End.X - point.X) < Float.EPSILON)
            {
                return ((point.Y - Start.Y) * (point.Y - End.Y))
                    <= Float.EPSILON;
            }
            return (point.X - Start.X) * (point.X - End.X) <= Float.EPSILON;
        }

        /// <summary>
        /// Check if an array of points is inside the segment
        /// </summary>
        /// <returns><c>true</c>, if inside was ised, <c>false</c> 
        /// otherwise.</returns>
        /// <param name="points">Point.</param>
        public bool IsInside(Point[] points)
        {
            if (points == null || points.Length == 0)
            {
                return false;
            }
            return points.All(IsInside);
        }

        /// <summary>
        /// Calculates the unit vector for this segment
        /// </summary>
        /// <returns>The unit.</returns>
        public Vector Unit()
        {
            return new Vector(End.X - Start.X, End.Y - Start.Y).Unit();
        }

        /// <summary>
        /// Returns the distance between the start and end points of the segment 
        /// </summary>
        /// <returns>The distance.</returns>
        public float Distance()
        {
            return Point.Distance(Start, End);
        }

        /// <summary>
        /// Calculate the angle for a segment
        /// </summary>
        /// <returns>The degrees.</returns>
        public float Degrees()
        {
            float degree = 0;
            var arctan = Slope;
            var dy = this.End.Y - this.Start.Y;
            var dx = this.End.X - this.Start.X;

            if (arctan != null && arctan != 0) {
                degree = (float)
                    (Math.Atan(Math.Abs(arctan.Value)) * 180 / Math.PI);

                if (dx < 0 && dy >= 0) { degree += 90; }
                if (dx <= 0 && dy < 0) { degree += 180; }
                if (dx > 0 && dy < 0) { degree += 270; }
            } else {
                if (arctan == null) {
                    degree = dy >= 0 ? 90 : 270;
                } else {
                    degree = dx >= 0 ? 0 : 180;
                }
            }
            return degree;
        }

        /// <summary>
        /// Intersection between two segments
        /// </summary>
        /// <returns>The intersect.</returns>
        /// <param name="withSegment">With segment.</param>
        public HashSet<Point> Intersect(Segment withSegment)
        {
            Point[] ret = new Point[0];
            var v = withSegment.Start;
            var u = withSegment.End;
            var bl = this.Intercept;
            var ml = this.Slope;

            if (withSegment.Slope == null)
            { //vertical edge
                if (ml == null) { //vertical line
                    if (Math.Abs(this.Start.X - v.X) < Float.EPSILON) {
                        // Same X - colinear
                        ret = new Point[] { this.Start, this.End, v, u };
                    }
                } else {
                    var y = (float)
                        Math.Round(ml.Value * v.X + bl.Value, Float.Decimals);
                    ret = new Point[] { new Point(v.X, y) };
                }
            } else {
                if (ml == null) { //vertical line
                    var y = (float)
                        Math.Round((withSegment.Slope.Value * Start.X) +
                        withSegment.Intercept.Value, Float.Decimals);
                    ret = new Point[] { new Point(Start.X, y) };

                } else if (Math.Abs(ml.Value - withSegment.Slope.Value)
                        < Float.EPSILON) {
                    //same slope, non vertical
                    if (Math.Abs(bl.Value - withSegment.Intercept.Value)
                        < Float.EPSILON) {
                        //same intercept - colinear
                        ret = new Point[] { this.Start, this.End, v, u };
                    }
                } else { // non vertical. different slopes
                    var x = (float)Math.Round(
                        (bl.Value - withSegment.Intercept.Value) /
                        (withSegment.Slope.Value - ml.Value),
                        Float.Decimals);
                    var y = (float)Math.Round(ml.Value * x + bl.Value,
                                              Float.Decimals);
                    ret = new Point[] { new Point(x, y) };
                }
            }
            return new HashSet<Point>(
                ret.Where(pt => IsInside(pt) 
                          && withSegment.IsInside(pt)));
        }

        /// <summary>
        /// Calculates the projection of a point on this segment
        /// </summary>
        /// <returns>The projection.</returns>
        /// <param name="point">Point.</param>
        public Point Projection(Point point)
        {
            var s = Slope;
            if (s != null) {
                var x = (point.X + (point.Y - Start.X) * s 
                         + Start.X * s * s) / (s * s + 1);
                var y = s * x + Intercept.Value;
                return new Point(x.Value, y.Value);
            }
            return new Point(this.Start.X, point.Y);
        }
    }
}
