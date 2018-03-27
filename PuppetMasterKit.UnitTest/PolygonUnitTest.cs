
using System.Linq;
using PuppetMasterKit.Geometry;
using NUnit.Framework;
using PuppetMasterKit.Polygons;
using System.Collections.Generic;

namespace PuppetMasterKit.UnitTest
{
    [TestFixture]
    public class PolygonUnitTest
    {
        [Test]
        public void TestMinMax()
        {
            var t2 = new Polygon(
                new Point[] {new Point(x: 7, y: 2),
                             new Point(x: 2, y: 10),
                             new Point(x: 2, y: 4),
                             new Point(x: 3, y: 16),
                             new Point(x: 3, y: 6),
                             new Point(x: 3, y: 6),
                             new Point(x: 10, y: 6),
                             new Point(x: 1, y: 5)});


            Assert.True(t2.MinX == 1);
            Assert.True(t2.MaxX == 10);
            Assert.True(t2.MinY == 2);
            Assert.True(t2.MaxY == 16);
        }
        [Test]
        public void TestLineIntersect_Set()
        {
            Point[] points = new Point[] {
                new Point(5,0),
                new Point(5,0)
            };
            var set = new HashSet<Point>(points);
            Assert.True(set.Count == 1);

        }
        //
        [Test]
        public void TestLineIntersect_3()
        {
            //1. Parallel and vertical M=NIL, ML=NIL
            var t1 = new Polygon(new Point[]{ new Point(x: 5, y: 0),
                                               new Point(x: 5, y: 10) });

            var r1 = t1.Intersect(new Segment(new Point(x: 6, y: 0),
                                              new Point(x: 6, y: 20)), 0);
            Assert.True(r1.Count == 0);

            //2. colinear
            var r2 = t1.Intersect(new Segment(new Point(x: 5, y: 0),
                                              new Point(x: 5, y: 20)), 0);
            Assert.True(r2.Count == 2);

            //3. vertical M=NIL, ML <> NIL
            var r3 = t1.Intersect(new Segment(new Point(x: 0, y: 0),
                                              new Point(x: 10, y: 10)), 0);

            Assert.True(r3.Count == 1 && r3.First().X == 5
                        && r3.First().Y == 5);

            // 4.vertical M<> NIL, ML = NIL
            var t2 = new Polygon(new Point[]{
                new Point(x: 0, y: 0),
                new Point(x: 10, y: 10) });
            var r4 = t2.Intersect(new Segment(new Point(x: 5, y: 0),
                                              new Point(x: 5, y: 10)), 0);

            Assert.True(r3.Count == 1 && r3.First().X == 5
                        && r3.First().Y == 5);

            //5. parallel. same slope
            var r5 = t2.Intersect(new Segment(new Point(x: 0, y: -1),
                                              new Point(x: 10, y: 9)), 0);

            Assert.True(r5.Count == 0);

            //6. colinear M = ML
            var r6 = t2.Intersect(new Segment(new Point(x: 0, y: 0),
                                              new Point(x: 10, y: 10)), 0);

            Assert.True(r6.Count == 2);

            //7. M <> ML non nil
            var r7 = t2.Intersect(new Segment(new Point(x: 5, y: 0),
                                              new Point(x: 0, y: 5)), 0);

            Assert.True(r7.Count == 1);
        }
        [Test]
        public void TestPolyIntersect1()
        {
            var t1 = new Polygon(new Point[]{
            new Point(x: 1, y: 1),
            new Point(x: 1, y: 4),
            new Point(x: 3, y: 4),
            new Point(x: 2, y: 2),
            new Point(x: 3, y: 1)
            });

            var intp = t1.Intersect(new Segment(new Point(x: 2.5f, y: 1),
                                                new Point(x: 2.5f, y: 5)));

            Assert.True(intp.Length == 4);
        }

        [Test]
        public void TestIntersectWithPolyNone()
        {
            var poly1 = new Polygon(new Point[]{
            new Point(x: 1, y: 1),
            new Point(x: 1, y: 3),
            new Point(x: 2, y: 2)
            });

            var poly2 = new Polygon(new Point[]{
            new Point(x: 1, y: 4),
            new Point(x: 1, y: 5),
            new Point(x: 2, y: 5)
            });

            var pts = poly1.Intersect(poly2, excludeVertices: false);

            Assert.True(pts.Length == 0);
        }

        [Test]
        public void TestIntersectWithPoly2()
        {
            var poly1 = new Polygon(new Point[]{
            new Point(x: 1, y: 1),
            new Point(x: 1, y: 4),
            new Point(x: 3, y: 3)
            });

            var poly2 = new Polygon(new Point[]{
            new Point(x: 1, y: 2),
            new Point(x: 1, y: 5),
            new Point(x: 3, y: 5)
            });

            var pts = poly1.Intersect(poly2, excludeVertices: false);
            Assert.True(pts.Count() == 3);
        }

        [Test]
        public void TestPointInsidePoly1()
        {
            var poly1 = new Polygon(new Point[]{
                new Point(x: 0, y: 0),
                new Point(x: 0, y: 5),
                new Point(x: 5, y: 5),
                new Point(x: 5, y: 0)
            });

            var t1 = poly1.IsPointInside(new Point(x: 2, y: 2));
            Assert.True(t1 == true);

            var t2 = poly1.IsPointInside(point: new Point(x: 6, y: 2));
            Assert.True(t2 == false);

            var t3 = poly1.IsPointInside(point: new Point(x: 5, y: 5));
            Assert.True(t3 == true);

            var t4 = poly1.IsPointInside(point: new Point(x: -1, y: 5));
            Assert.True(t4 == false);

            var t5 = poly1.IsPointInside(point: new Point(x: -1, y: 3));
            Assert.True(t5 == false);

            var t6 = poly1.IsPointInside(point: new Point(x: 2, y: 5));
            Assert.True(t6 == true);

            //diamond shape
            var poly2 = new Polygon(new Point[]
            { new Point(x: 2, y: 0),
              new Point(x: 0, y: 2),
              new Point(x: 2, y: 4),
              new Point(x: 4, y: 2)});
            var t7 = poly2.IsPointInside(new Point(x: 0, y: 0));
            Assert.True(t7 == false);

            //non convex
            var nonConvex = new Polygon(new Point[] {
                new Point(x: 1, y: 2),
                new Point(x: 2, y: 4),
                new Point(x: 3, y: 3),
                new Point(x: 4, y: 5),
                new Point(x: 5, y: 3),
                new Point(x: 4, y: 1),
                new Point(x: 3, y: 2),
                new Point(x: 2, y: 1)
            });
            Assert.True(nonConvex.IsPointInside(point: new Point(x: 2, y: 2)));
            Assert.False(nonConvex.IsPointInside(point: new Point(x: 3, y: 4)));
            Assert.True(nonConvex.IsPointInside(point: new Point(x: 4, y: 1)));
        }

        [Test]
        public void TestAdjacent1()
        {
            var poly1 = new Polygon(new Point[]{
                new Point(x: 1, y: 1),
                new Point(x: 1, y: 3),
                new Point(x: 3, y: 3),
                new Point(x: 3, y: 1)
            });

            var poly2 = new Polygon(new Point[]{
                new Point(x: 3, y: 2),
                new Point(x: 3, y: 4),
                new Point(x: 5, y: 4),
                new Point(x: 5, y: 2)
            });

            var poly3 = new Polygon(new Point[]{
                new Point(x: 4, y: 0),
                new Point(x: 4, y: 1),
                new Point(x: 5, y: 1),
                new Point(x: 5, y: 0)
            });

            var poly4 = new Polygon(new Point[]{
                new Point(x: 3, y: 0),
                new Point(x: 3, y: 1),
                new Point(x: 4, y: 1),
                new Point(x: 4, y: 0)
            });

            var poly5 = new Polygon(new Point[]{
                new Point(x: 3, y: 0),
                new Point(x: 3, y: 1.2f),
                new Point(x: 4, y: 1.2f),
                new Point(x: 4, y: 0)
            });

            Assert.True(poly1.IsAdjacent(poly2) == true);
            Assert.True(poly1.IsAdjacent(poly3) == false);
            Assert.True(poly1.IsAdjacent(poly4) == false);
            Assert.True(poly1.IsAdjacent(poly5) == true);
        }

        [Test]
        public void TestCutExample1()
        {
            var P = new Polygon(new Point[] {
                new Point(x: 0, y: 0),
                new Point(x: 0, y: 4),
                new Point(x: 4, y: 4),
                new Point(x: 4, y: 0)});

            var C = new Polygon(new Point[]{
                 new Point(x: 2, y: 0),
                 new Point(x: 0, y: 2),
                 new Point(x: 2, y: 4),
                 new Point(x: 4, y: 2)});

            var res = P.CutPolygon(C);

            Assert.True(res.Count() == 4);
        }

        [Test]
        public void TestCutExample2()
        {
            var P = new Polygon(new Point[]{
                new Point(x: 3, y: 11),
                new Point(x: 9, y: 11),
                new Point(x: 9, y: 3),
                new Point(x: 3, y: 3)});

            var C = new Polygon(new Point[] {
                new Point(x: 12, y: 7),
                new Point(x: 8, y: 0),
                new Point(x: 3, y: 8),
                new Point(x: 9, y: 12)});

            var res = P.CutPolygon(C);

            Assert.True(res.Count() == 2);
        }

        [Test]
        public void TestCutExample3()
        {
            var P = new Polygon(new Point[] {
                new Point(x: 2, y: 1),
                new Point(x: 2, y: 7),
                new Point(x: 4, y: 7),
                new Point(x: 4, y: 1)});
        
            var C1 = new Polygon(new Point[] {
                new Point(x: 2, y: 2),
                new Point(x: 2, y: 6),
                new Point(x: 4, y: 5)});
            
            var C2 = new Polygon(new Point[]{
                new Point(x: 2, y: 1),
                new Point(x: 2, y: 7),
                new Point(x: 4, y: 7),
                new Point(x: 4, y: 1)});

            var C3 = new Polygon(new Point[]{
                new Point(x: 2, y: 1),
                new Point(x: 2, y: 7),
                new Point(x: 4, y: 7)});

            var res = P.CutPolygon(C1);
            Assert.True(res.Count() == 2);

            res = P.CutPolygon(C2);
            Assert.True(res.Count() == 0);

            res = P.CutPolygon(C3);
            Assert.True(res.Count() == 1);
        }

        [Test]
        public void TestCutExample4()
        {
            var P = new Polygon(new Point[]{
             new Point(x: 3.0526f, y: 1),
             new Point(x: 3.0526f, y: 2.0526f),
             new Point(x: 4.5f, y: 3.5f),
             new Point(x: 4.5f, y: 1)});
        
        
            var C1 = new Polygon(new Point[]{
             new Point(x: 2, y: 5),
             new Point(x: 2, y: 7),
             new Point(x: 7, y: 4),
             new Point(x: 7, y: 2)});

            var res = P.CutPolygon(C1);
            Assert.True(res.Count() == 0);
        }

        [Test]
        public void TestIsInside()
        {
            var p1 = new Polygon(new Point[]{
                new Point(x: 1, y: 1),
                new Point(x: 1, y: 4),
                new Point(x: 4, y: 4),
                new Point(x: 4, y: 1),
                });

            var o1 = new Polygon(new Point[]{
                new Point(x: 1, y: 1),
                new Point(x: 1, y: 4),
                new Point(x: 4, y: 4),
                new Point(x: 4, y: 1),
                });

            Assert.True(p1.IsPolygonInside(o1));
        }
    }
}
