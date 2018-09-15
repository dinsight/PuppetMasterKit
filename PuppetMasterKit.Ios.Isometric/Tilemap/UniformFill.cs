using System;
using System.Collections.Generic;
using PuppetMasterKit.AI;
using PuppetMasterKit.Graphics.Geometry;
using SpriteKit;

namespace PuppetMasterKit.Ios.Isometric.Tilemap
{
  public static class UniformFill
  {
    private static Random random = new Random(Guid.NewGuid().GetHashCode());

    public static IEnumerable<Point> Fill(float startX,
                                          float startY,
                                          float endX,
                                          float endY,
                                          float density)
    {
      var p = 0.4f;
      var step = (endX - startX) / density;

      var i = startX + step * 0.5f;
      while (i <= endX) {
        var j = startY + step * 0.5f;
        while (j <= endY) {
          var x = i + step * GetRandom(-p,p);
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
    private static float GetRandom(float a, float b)
    {
      var f = (float)random.NextDouble();
      return f * (b - a) + a;
    }
  }
}
