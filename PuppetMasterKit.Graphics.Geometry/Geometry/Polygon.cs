using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.Graphics.Geometry
{
    public class Polygon
    {
        public enum Direction
        {
            Clockwise,
            Counterclockwise
        }

        public float MinX { get; set; }

        public float MaxX { get; set; }

        public float MinY { get; set; }

        public float MaxY { get; set; }

        public List<Point> Points { get; set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return this.Points.Count; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Polygons.Polygon"/> class.
        /// </summary>
        /// <param name="points">The polygon's points in <b>clockwise</b> sequence</param>
        public Polygon(Point[] points)
        {
            this.Points = new List<Point>(points);
            CalcMinMax();
        }

        /// <summary>
        /// Gets the <see cref="T:PuppetMasterKit.Polygons.Polygon"/> with the specified n.
        /// </summary>
        /// <param name="n">N.</param>
        public Point this[int n]
        {
            get { 
                if( n>=0 && n < Count ) {
                    return Points[n];
                }
                return null;
            }
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="lhs">The first <see cref="PuppetMasterKit.Polygons.Polygon"/> to compare.</param>
        /// <param name="rhs">The second <see cref="PuppetMasterKit.Polygons.Polygon"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Polygon lhs, Polygon rhs)
        {
            if (!Object.ReferenceEquals(lhs,null) 
                && !Object.ReferenceEquals(rhs,null))
            {
                var set1 = new HashSet<Point>(lhs.Points);
                var set2 = new HashSet<Point>(rhs.Points);
                var intersection = set1.Intersect(set2);
                return intersection.Count() == set1.Count;
            }
            return false;
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        /// <param name="lhs">The first <see cref="PuppetMasterKit.Polygons.Polygon"/> to compare.</param>
        /// <param name="rhs">The second <see cref="PuppetMasterKit.Polygons.Polygon"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Polygon lhs, Polygon rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Equality function
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:PuppetMasterKit.Polygons.Polygon"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="T:PuppetMasterKit.Polygons.Polygon"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Polygon) {
                return this == (Polygon)obj;
            }
            return false;
        }

        /// <summary>
        /// Hase Code function
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            checked
            {
                var val = 31;
                foreach (var x in Points)
                {
                    val = val = (val * 17) & +x.GetHashCode();
                }
                return val;
            }
        }

		/// <summary>
		/// Append the specified point.
		/// </summary>
		/// <returns>The append.</returns>
		/// <param name="point">Point.</param>
        public void Append(Point point)
        {
            Points.Add(point);
            CalcMinMax();
        }

        /// <summary>
        /// Insert the specified point at index.
        /// </summary>
        /// <returns>The insert.</returns>
        /// <param name="point">Point.</param>
        /// <param name="at">At.</param>
        public void Insert(Point point,int index)
        {
            Points.Insert(index, point);
            CalcMinMax();
        }

        /// <summary>
        /// Calculates the minimum max.
        /// </summary>
        private void CalcMinMax()
        {
            MinX = Points.Min(a => a.X);
            MaxX = Points.Max(a => a.X);
            MinY = Points.Min(a => a.Y);
            MaxY = Points.Max(a => a.Y);
        }

        /// <summary>
        /// Get the polygon edge starting at node index "atIndex"
        /// </summary>
        /// <returns>The edge.</returns>
        /// <param name="atIndex">At index.</param>
        public Segment GetEdge(int atIndex)
        {
            if (atIndex < Count && Count > 1) {
                var u = Points[atIndex];
                var v = atIndex == Points.Count - 1 ?
                        Points[0] : Points[atIndex + 1];
                return new Segment(u, v);
            }
            return null;
        }

        /// <summary>
        /// Intersection between a line and the polygon edge starting at index
        /// </summary>
        /// <returns>The intersect.</returns>
        /// <param name="segment">Segment.</param>
        /// <param name="index">Index.</param>
        public ISet<Point> Intersect(Segment segment, int index) 
        {
            if (index < Points.Count) {
                var edge = GetEdge(index);
                return segment.Intersect(edge);
            }
            return null;
        }

        /// <summary>
        /// Intersects a line with the current polygon
        /// </summary>
        /// <returns>The intersect.</returns>
        /// <param name="segment">Segment.</param>
        public Point[] Intersect(Segment segment) 
        {
            var intersections = new HashSet<Point>();
            for (int index = 0; index < Points.Count; index++)
            {
                var intp = Intersect(segment, index);
                intersections.UnionWith(intp);
            }
            return intersections.ToArray();
        }

        /// <summary>
        /// Intersect with another polygon
        /// </summary>
        /// <returns>The intersect.</returns>
        /// <param name="polygon">Polygon.</param>
        /// <param name="excludeVertices">If set to <c>true</c> exclude vertices.</param>
        public Point[] Intersect(Polygon polygon, bool excludeVertices = false)  
        {
            var result = new HashSet<Point>();
            for (int index = 0; index < polygon.Count; index++)
            {
                var inputEdge = polygon.GetEdge(index);
                if (inputEdge != null) {
                    var pts = Intersect(inputEdge);
                    if (pts != null) {
                        result.UnionWith(pts);
                    }
                }
            }

            if (excludeVertices){
                return result
                    .Where(p => Points.Exists(q => 
                            Math.Abs(p.X - q.X) < Float.EPSILON &&
                            Math.Abs(p.Y - q.Y) < Float.EPSILON) || 
                        polygon.Points.Exists(q => 
                            Math.Abs(p.X - q.X) < Float.EPSILON &&
                            Math.Abs(p.Y - q.Y) < Float.EPSILON ))
                    .ToArray();
            }
            return result.ToArray();
        }

        /// <summary>
        /// Calculate the angle for a given edge starting at index "index"
        /// </summary>
        /// <returns>The degrees.</returns>
        /// <param name="index">Index.</param>
        private float Degrees(int index) 
        {
            var edge = GetEdge(index);
            return edge.Degrees();
        }

        /// <summary>
        /// Determine the direction of the next edge relative to the current edge
        /// </summary>
        /// <returns>The direction.</returns>
        /// <param name="prev">Previous.</param>
        /// <param name="deg">Deg.</param>
        private Direction? GetDirection(float prev, float deg)
        {
            if(prev + 180 < 360) {
                if( deg > prev && deg < prev + 180) {
                    return Direction.Counterclockwise;
                }
            } else {
                if( (deg > prev && deg < 360) || deg < prev - 180) {
                    return Direction.Counterclockwise;
                }
            }

            if(prev - 180 > 0) {
                if( deg < prev && deg > prev - 180) {
                    return Direction.Clockwise;
                }
            } else {
                if ((deg < prev && deg >= 0) || deg > prev - 180) {
                    return Direction.Clockwise;
                }
            }
            return null;
        }

        /// <summary>
        /// Check if the poligon is convex
        /// </summary>
        /// <returns><c>true</c>, if convex was ised, <c>false</c> otherwise.</returns>
        public bool IsConvex() 
        {
            if (Points.Count <= 2)
                return false;

            var prevDegree = Degrees(0);
            var degree = Degrees(1);
            var direction = GetDirection(prevDegree, degree);

            for (int index = 2; index < Points.Count; index++)
            {
                prevDegree = degree;
                degree = Degrees(index);
                var dir = GetDirection(prevDegree, degree);
                if (dir != null && dir != direction){
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Calculated the center point for the polygon
        /// </summary>
        /// <returns>The centroid.</returns>
        public Point Centroid()
        {
            var xc = 0f;
            var yc = 0f;

            foreach (var p in Points)
            {
                xc += p.X;
                yc += p.Y;
            }

            xc = xc / Points.Count;
            yc = yc / Points.Count;
            return new Point(xc, yc);
        }

        /// <summary>
        /// Returns the sign of an edge. 1- upward, -1 - downward, 0 - horizontal
        /// </summary>
        /// <returns>The sign.</returns>
        /// <param name="edge">Edge.</param>
        private int Sign(Segment edge) 
        {
            var diff = edge.End.Y - edge.Start.Y;
            return diff > 0 ? 1 : (diff < 0 ? -1 : 0);
        }

        /// <summary>
        /// Checks if a point is inside the polygon
        /// http://geomalgorithms.com/a03-_inclusion.html
        /// </summary>
        /// <returns><c>true</c>, if point inside was ised, <c>false</c> otherwise.</returns>
        /// <param name="point">Point.</param>
        public bool IsPointInside(Point point) 
        {
            var ray = new Segment(point, new Point(this.MaxX + 10, point.Y));
            var count = 0;

            for (int index = 0; index < Count; index++)
            {
                var edge = GetEdge(index);
                var sign = Sign(edge);
                var intp = edge.Intersect(ray);

                if (intp.Count != 0) {
                    if (intp.Contains(point))
                        return true;
                
                    if( sign > 0 && intp.First() != edge.End) {
                        count += 1;
                    }
                    if(sign < 0 && intp.First() != edge.Start ) {
                        count += 1;
                    }
                }
            }
            return count % 2 != 0;
        }

        /// <summary>
        /// Check if poly is inside the current polygon
        /// </summary>
        /// <returns><c>true</c>, if polygon inside was ised, <c>false</c> otherwise.</returns>
        /// <param name="poly">Poly.</param>
        public bool IsPolygonInside(Polygon poly) 
        {
            var inside = true;
            foreach (var x in Points)
            {
                inside = inside && poly.IsPointInside(x);
            }
            return inside;
        }

        /// <summary>
        /// Return the current polygon's points together with the intersection points
        /// </summary>
        /// <returns>The poly cut points.</returns>
        /// <param name="polygon">Polygon.</param>
        /// <param name="withIntersect">With intersect.</param>
        private CutPoint[] GetPolyCutPoints(Polygon polygon, 
                                            Polygon withIntersect ) 
        {
            var polySet = new List<CutPoint>();

            for (int i = 0; i < polygon.Points.Count; i++)
            {
                var point = polygon.Points[i];
                var edgeP = polygon.GetEdge(i);
                var currPoint = new CutPoint(point);

                polySet.Add(currPoint);

                var intersections = new HashSet<Point>();
                for (int j = 0; j < withIntersect.Points.Count; j++)
                {
                    var edgeC = withIntersect.GetEdge(j);
                    if (edgeC != null){
                        var edgeIntersect = edgeP.Intersect(edgeC);
                        intersections.UnionWith(edgeIntersect);
                    }
                }

                var sinter = point.SortByDistance(intersections.ToArray());
                foreach (var item in sinter)
                {
                    if (item == currPoint.Point){
                        currPoint.IsWaypoint = true;
                    } else {
                        polySet.Add(new CutPoint(item, true));
                    }
                }
            }

            return polySet.Deduplicate().ToArray();
        }

        /// <summary>
        /// Cuts the current polygon using the shape of another polygon
        /// Returns the list of resulting polygons
        /// </summary>
        /// <returns>The polygon.</returns>
        /// <param name="startAt">Start at.</param>
        /// <param name="P">P.</param>
        /// <param name="C">C.</param>
        private Polygon CutPolygon(int startAt, CutPoint[] P, CutPoint[] C)
        {
            var direction = 1; //forward
            var index = startAt;
            var thePoints = P;
            var startPoint = thePoints[index];
            var currPoint = startPoint;
            var result = new List<Point>();

            do
            {
                result.Add(currPoint.Point);
                currPoint.IsVisited = true;

                if (currPoint.IsWaypoint) {
                    //if this is a waypoint, switch directions
                    direction = -direction;
                    thePoints = thePoints == P ? C : P;
                    index = Array.IndexOf(thePoints, currPoint);
                }

                index += direction;

                //wrap around for backward dir
                if( index < 0 && direction < 0) {
                    index = thePoints.Count() - 1;
                }

                //wrap around for forward dir
                if( index >= thePoints.Count() && direction > 0) {
                    index = 0; 
                }
                currPoint = thePoints[index];
                    
            } while (currPoint != startPoint);

            return new Polygon(result.ToArray());
        }

        /// <summary>
        /// Cuts this polygon alongs the lines of polygon P. Returns the resulting polygons
        /// </summary>
        /// <returns>The polygon.</returns>
        /// <param name="poly">Poly.</param>
        public Polygon[] CutPolygon(Polygon poly)
        {
            var P = GetPolyCutPoints(this, poly);
            var C = GetPolyCutPoints(poly, this);
            var result = new List<Polygon>();

            var waypointsC = C.Where(x => x.IsWaypoint);
            if (waypointsC.Count() == 0) {
                // there are no intersection points bewteen the two polys
                return new Polygon[0];
            } else if (waypointsC.Count() == 1) {
                //if there's only one waypoint, check if the cut is outside or inside polygon P
                //it is sufficient to test just one more point besides the waypoint to determine
                var pc = C.Where(x => !x.IsWaypoint).FirstOrDefault();
                if (pc!=null) {
                    if (!this.IsPointInside(pc.Point)) { 
                        return new Polygon[0];
                    }
                }
            }

            for (int index = 0; index < P.Count(); index++)
            {
                if(!P[index].IsVisited && !P[index].IsWaypoint 
                   && !poly.IsPointInside(P[index].Point)) {

                    var cut = CutPolygon(index, P, C);
                    if (cut!=null) {
                        result.Add(cut);
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Check if the polygons are adjacent
        /// </summary>
        /// <returns><c>true</c>, if adjacent was ised, <c>false</c> otherwise.</returns>
        /// <param name="p">P.</param>
        public bool IsAdjacent(Polygon p)
        {
            //Test only if the plygons' minX(Y) or maxX(Y) are overlapping
            if (!this.MinX.IsBetween(p.MinX, p.MaxX)
                || !MaxX.IsBetween(p.MinX, p.MaxX)
                || !MinY.IsBetween(p.MinY, p.MaxY)
                || !MaxY.IsBetween(p.MinY, p.MaxY)
                || !p.MinX.IsBetween(MinX, MaxX)
                || !p.MaxX.IsBetween(MinX, MaxX)
                || !p.MinY.IsBetween(MinY, MaxY)
                || !p.MaxY.IsBetween(MinY, MaxY)
               )
            {
                for (int i = 0; i < Points.Count; i++)
                {
                    var edge1 = this.GetEdge(i);
                    if (edge1!=null) {
                        for (int j = 0; j < p.Points.Count; j++)
                        {
                            var edge2 = p.GetEdge(j);
                            if (edge2!=null) {
                                if (edge1.Intersect(edge2).Count > 1) { 
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

    }
}
