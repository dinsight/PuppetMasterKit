using System;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.Graphics.Sprites
{
  public interface ICoordinateMapper
  {
    Point FromScene(Point point);

    Point ToScene(Point point);

    String ToSceneOrientation(String orientation);
  }
}
