using System;
using System.Collections.Generic;
using System.Linq;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Ios.Tiles.Tilemap.Helpers;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Utility.Extensions;
using SpriteKit;

namespace PuppetMasterKit.Ios.Tiles.Tilemap
{
  public class RegionFill
  {
    private static Random randomDist = new Random(Guid.NewGuid().GetHashCode());

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
                           SKNode layer, Random random)
    {
      

      var regionsToFill = regions.Where(x => x.RegionFill == regionFillCode);

      var defs = group.Rules
                      .SelectMany(a => a.TileDefinitions)
                      .SelectMany(b => b.Textures).ToList();
      //local fn
      float GetRandom(float a, float b)
      {
        var f = (float)random.NextDouble();
        return f * (b - a) + a;
      }
      var randOcc = random;
      regionsToFill.ForEach(reg=> {
        reg.TraverseRegion((row, col, type) => { 
          if(type == TileType.Plain) 
          {
            if (defs.Any()) {
              var texture = defs[random.Next(0, defs.Count())];
              var r = GetRandom(-0.2f, 0.2f);
              densityFactor = densityFactor >= 1 ? 0 : densityFactor;
              var occ = randOcc.Next(0, 1 + (int)(densityFactor * 100));
              if (occ == 0) {
                texture.SetTexture(layer, 
                  row * tileSize + tileSize / 2f + tileSize * r, 
                  col * tileSize + tileSize / 2f + tileSize * r);
              }
            }
          }
        }, false);
      });
    }
  }
}
