using System;
using System.Linq;
using NUnit.Framework;
using PuppetMasterKit.AI;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Terrain;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Terrain.Noise;

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

    [Test]
    public void Subregion() {
      int[,] map = {
        {0, 0,  0,   0,   0,   0,   0,  0, 0, 0},
        {0, 0,  0,   0,   0,   0,   0,  0, 0, 0},
        {0, 0, 'A', 'A', 'A',  0,   0,  0, 0, 0},
        {0, 0, 'A', 'A', 'A', 'A',  0,  0, 0, 0},
        {0, 0, 'A', 'A', 'A', 'A',  0,  0, 0, 0},
        {0, 0, 'A', 'A', 'A', 'A', 'A', 0, 0, 0},
        {0, 0,  0,  'A', 'A', 'A', 'A', 0, 0, 0},
        {0, 0,  0,   0,   0,  'A', 'A', 0, 0, 0},
        {0, 0,  0,   0,   0,   0,   0,  0, 0, 0},
        {0, 0,  0,   0,   0,   0,   0,  0, 0, 0},
      };
      float[,] gradient = {
          {0.1f   , 0.2f , 0.1f , 0.1f , 0.1f},
          {0.2f   , 0.8f , 1.0f , 0.3f , 0.1f},
          {0.4f   , 0.6f , 0.9f , 0.7f , 0.1f},
          {0.1f   , 0.2f , 0.4f , 1.0f , 0.1f},
          {0.1f   , 0.1f , 0.1f , 0.4f , 0.1f},
        };

      float GRAD = gradient.GetLength(0);
      float M = map.GetLength(0)-1;
      var noiseAmplitude = 7.0f;
      var regions = Region.ExtractRegions(map);
      var region1 = regions.First(x => x.RegionFill == 'A');
      //Print(region1, map.GetLength(0),map.GetLength(0));

      var perlin = new Perlin(gradient);

      foreach (var item in region1.Tiles) {
        var scol = (item.Col - region1.MinCol) * (GRAD / M);
        var srow = (item.Row - region1.MinRow) * (GRAD / M);
        var n = perlin.Noise(scol, srow) * noiseAmplitude;
        if (n >= -1 && n < -0.5 || n < -1) {
          region1[item.Row, item.Col] = '@';
        }
        if (n >= -0.5 && n < 0) {
          region1[item.Row, item.Col] = '+';
        }
        if (n >= 0 && n < 0.5) {
          region1[item.Row, item.Col] = '-';
        }
        if (n >= 0.5 && n <= 1 || n > 1) {
          region1[item.Row, item.Col] = '~';
        }
      }

      Print(region1, map.GetLength(0), map.GetLength(0));

      var inner = Region.ExtractRegions(region1);
      var high = inner.First(x => x.RegionFill == '@');

      System.Diagnostics.Debug.WriteLine("");
      System.Diagnostics.Debug.WriteLine("");

      Print(high, map.GetLength(0), map.GetLength(0));
    }

    private void Print(Region region, int rows, int cols) {
      for (int row = 0; row < rows; row++) {
        for (int col = 0; col < cols; col++) {
          if (region[row, col] != null) {
            System.Diagnostics.Debug.Write((char)region[row, col]);
          } else {
            System.Diagnostics.Debug.Write('.');
          }

        }
        System.Diagnostics.Debug.WriteLine("");
      }
    }
  }
}
