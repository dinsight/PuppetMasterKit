using System;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI
{
  public class CircularObstacle : Obstacle
  {
    private Point center;
    private float radius;

    public float Radius => radius;

    public Point Center => center;


    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.CircularObstacle"/> class.
    /// </summary>
    /// <param name="center">Center.</param>
    /// <param name="radius">Radius.</param>
    public CircularObstacle(Point center, float radius)
    {
      this.center = center;
      this.radius = radius;
    }

    /// <summary>
    /// Ises the inside.
    /// </summary>
    /// <returns><c>true</c>, if inside was ised, <c>false</c> otherwise.</returns>
    /// <param name="point">Point.</param>
    public override bool IsInside(Point point)
    {
      return Point.Distance(point, center) <= Radius;
    }

    /// <summary>
    /// Gets the center point.
    /// </summary>
    /// <returns>The center point.</returns>
    public override Point GetCenterPoint()
    {
      return center;
    }

    /// <summary>
    /// Gets the force radius.
    /// </summary>
    /// <returns>The force radius.</returns>
    public override float GetForceRadius()
    {
      return Radius;
    }
  }
}
