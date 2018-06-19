using System;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI
{
  public class PolygonalObstacle : Obstacle
  {
    public readonly Polygon Polygon;

    public Point Centroid { get => Polygon.Centroid(); }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.PolygonalObstacle"/> class.
    /// </summary>
    /// <param name="points">Points.</param>
    public PolygonalObstacle(params Point[] points)
    {
      this.Polygon = new Polygon(points);
    }

    /// <summary>
    /// Gets the center point.
    /// </summary>
    /// <returns>The center point.</returns>
    public override Point GetCenterPoint()
    {
      return Centroid;
    }

    /// <summary>
    /// Ises the inside.
    /// </summary>
    /// <returns><c>true</c>, if inside was ised, <c>false</c> otherwise.</returns>
    /// <param name="point">Point.</param>
    public override bool IsInside(Point point)
    {
      return Polygon.IsPointInside(point);
    }
  }
}
