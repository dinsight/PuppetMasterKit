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

    public override Point GetCenterPoint()
    {
      return Centroid;
    }
  }
}
