﻿using System;
using SpriteKit;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI;
using PuppetMasterKit.Utility;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.Ios.Isometric.Fill
{
  public class RegionFill
  {
    private static Random randomDist = new Random(Guid.NewGuid().GetHashCode());

    private static Random randomFill = new Random(Guid.NewGuid().GetHashCode());

    /// <summary>
    /// Fills the uniform.
    /// </summary>
    /// <returns>The uniform.</returns>
    /// <param name="startX">Start x.</param>
    /// <param name="startY">Start y.</param>
    /// <param name="endX">End x.</param>
    /// <param name="endY">End y.</param>
    /// <param name="density">Density.</param>
    public static IEnumerable<Point> FillUniform(float startX,
                                          float startY,
                                          float endX,
                                          float endY,
                                          float density)
    {
      var p = 0.4f;
      var step = (endX - startX) / density;

      //local fn
      float GetRandom(float a, float b)
      {
        var f = (float)randomDist.NextDouble();
        return f * (b - a) + a;
      }

      var i = startX + step * 0.5f;
      while (i <= endX) {
        var j = startY + step * 0.5f;
        while (j <= endY) {
          var x = i + step * GetRandom(-p, p);
          var y = j + step * GetRandom(-p, p);
          if (x >= startX && y >= startY && x <= endX && y <= endY) {
            yield return new Point(x, y);
          }
          j += step;
        }
        i += step;
      }
    }

    /// <summary>
    /// Gets the random.
    /// </summary>
    /// <returns>The random.</returns>
    /// <param name="a">The alpha component.</param>
    /// <param name="b">The blue component.</param>
    //private static float GetRandom(float a, float b)
    //{
    //  var f = (float)randomDist.NextDouble();
    //  return f * (b - a) + a;
    //}

    /// <summary>
    /// Fills the region.
    /// </summary>
    /// <param name="regions">Regions.</param>
    /// <param name="tileSize">Tile size.</param>
    /// <param name="regionFillCode">Region fill code.</param>
    /// <param name="group">Group.</param>
    /// <param name="densityFactor">Density factor.</param>
    /// <param name="layer">Layer.</param>
    public static void Fill(IEnumerable<Region> regions, 
                           int tileSize,
                           int regionFillCode, 
                           SKTileGroup group, 
                           float densityFactor, 
                           SKNode layer)
    {
      

      var regionsToFill = regions.Where(x => x.RegionFill == regionFillCode);

      var defs = group.Rules
                      .SelectMany(a => a.TileDefinitions)
                      .SelectMany(b => b.Textures).ToList();

      regionsToFill.ForEach(x => {
        var density = densityFactor * x.MaxCol;
        var filler = FillUniform(0, 0, x.MaxCol + 1, x.MaxRow + 1, density);

        //var texture = defs[random.Next(0, defs.Count())];
        //layer.SetTexture(texture, 4 * tileSize, 0 * tileSize);

        foreach (var item in filler) {
          var col = (int)item.X;
          var row = (int)item.Y;
          if (x[row, col] != null) {
            //the point is inside the region
            if (defs.Any()) {
              var texture = defs[randomFill.Next(0, defs.Count())];
              texture.SetTexture(layer, item.Y * tileSize, item.X * tileSize);
            }
          }
        }
      });
    }
  }
}