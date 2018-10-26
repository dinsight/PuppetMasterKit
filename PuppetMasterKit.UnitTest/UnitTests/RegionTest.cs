using System;
using NUnit.Framework;
using PuppetMasterKit.AI;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Terrain;
using PuppetMasterKit.Terrain.Map;

namespace PuppetMasterKit.UnitTest.UnitTests
{
  [TestFixture]
  public class RegionTest
  {
    [Test]
    public void FillRegion_Test1()
    {
      var region = new Region(0);
      region.AddTile(1, 3);
      region.AddTile(2, 2);
      region.AddTile(3, 1);
      region.AddTile(3, 2);
      region.AddTile(4, 2);
      region.AddTile(4, 3);
      region.AddTile(5, 2);
      region.AddTile(6, 1);
      region.AddTile(6, 2);
      region.AddTile(7, 3);


      var expected = new GridCoord[]{
        new GridCoord(3,0), new GridCoord(4,0), new GridCoord(4,1), new GridCoord(5,1), new GridCoord(5,0),
        new GridCoord(6,0), new GridCoord(7,0), new GridCoord(7,1), new GridCoord(7,2), new GridCoord(8,2),
        new GridCoord(8,3), new GridCoord(8,4), new GridCoord(7,4), new GridCoord(6,4), new GridCoord(6,3),
        new GridCoord(5,3), new GridCoord(5,4), new GridCoord(4,4), new GridCoord(3,4), new GridCoord(3,3),
        new GridCoord(2,3), new GridCoord(2,4), new GridCoord(1,4), new GridCoord(0,4), new GridCoord(0,3),
        new GridCoord(0,2), new GridCoord(1,2), new GridCoord(1,1), new GridCoord(2,1), new GridCoord(2,0)
      };
      var contour = region.TraceContour();
      for (int i = 0; i < contour.Count; i++) {
        Assert.True(contour[i] == expected[i]);
      }
      foreach (var item in contour) {
        System.Diagnostics.Debug.WriteLine($"({item.Row},{item.Col})");
      }
    }

    [Test]
    public void FillRegion_Test2()
    {
      var region = new Region(0);
      region.AddTile(3, 1);
      region.AddTile(4, 2);
      region.AddTile(5, 2);
      region.AddTile(6, 3);

      var expected = new GridCoord[]{
        new GridCoord(3,0), new GridCoord(4,0), new GridCoord(4,1), new GridCoord(5,1), new GridCoord(6,1),
        new GridCoord(6,2), new GridCoord(7,2), new GridCoord(7,3), new GridCoord(7,4), new GridCoord(6,4),
        new GridCoord(5,4), new GridCoord(5,3), new GridCoord(4,3), new GridCoord(3,3), new GridCoord(3,2),
        new GridCoord(2,2), new GridCoord(2,1), new GridCoord(2,0)
      };
      var contour = region.TraceContour();
      for (int i = 0; i < contour.Count; i++) {
        Assert.True(contour[i] == expected[i]);
      }
      foreach (var item in contour) {
        System.Diagnostics.Debug.WriteLine($"({item.Row},{item.Col})");
      }
    }

    [Test]
    public void FillRegion_Test3()
    {
      var region = new Region(0);
      region.AddTile(1, 1);

      var expected = new GridCoord[]{
        new GridCoord(1,0), new GridCoord(2,0), new GridCoord(2,1), new GridCoord(2,2), new GridCoord(1,2),
        new GridCoord(0,2), new GridCoord(0,1), new GridCoord(0,0)
      };
      var contour = region.TraceContour();
      for (int i = 0; i < contour.Count; i++) {
        Assert.True(contour[i] == expected[i]);
      }
      foreach (var item in contour) {
        System.Diagnostics.Debug.WriteLine($"({item.Row},{item.Col})");
      }
    }
  }
}
