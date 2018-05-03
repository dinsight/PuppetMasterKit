using System;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI
{
  public abstract class Obstacle
  {
    /// <summary>
    /// Ises the inside.
    /// </summary>
    /// <returns><c>true</c>, if inside was ised, <c>false</c> otherwise.</returns>
    /// <param name="point">Point.</param>
    public virtual bool IsInside(Point point)
    {
      throw new NotImplementedException();
    }

    public virtual Point GetCenterPoint()
    {
      throw new NotImplementedException();
    }

    public virtual float GetForceRadius()
    {
      throw new NotImplementedException();
    }
  }
}
