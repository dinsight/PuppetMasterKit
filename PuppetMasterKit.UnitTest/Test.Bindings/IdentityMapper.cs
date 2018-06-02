using System;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;

namespace PuppetMasterKit.UnitTest.Test.Bindings
{
  public class IdentityMapper : ICoordinateMapper
  {
    public Point FromScene(Point point)
    {
      return point;
    }

    public Point ToScene(Point point)
    {
      return point;
    }

    public String ToSceneOrientation(String orientation)
    {
      return orientation;
    }
  }
}
