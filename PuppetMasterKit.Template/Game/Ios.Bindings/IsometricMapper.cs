using System;
using System.Linq;
using CoreGraphics;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Game.Ios.Bindings
{
  public class IsometricMapper : ICoordinateMapper
  {
    SKScene scene;

    public IsometricMapper(SKScene scene)
    {
      this.scene = scene;
    }

    public Point FromScene(Point point)
    {
      return new Point((2* point.Y + point.X)/2, (2* point.Y - point.X)/2) * -1;
    }

    public Point ToScene(Point point)
    {
      return new Point(point.X - point.Y, (point.X + point.Y)/2) * -1;
    }

    public String ToSceneOrientation(String orientation)
    {
      
      if(Orientation.N == orientation ){
        return Orientation.SE;
      }
      if (Orientation.NE == orientation) {
        return Orientation.S;
      }
      if (Orientation.E == orientation) {
        return Orientation.SW;
      }

      if (Orientation.SE == orientation) {
        return Orientation.W;
      }

      if (Orientation.S == orientation) {
        return Orientation.NW;
      }

      if (Orientation.SW == orientation) {
        return Orientation.N;
      }

      if (Orientation.W == orientation) {
        return Orientation.NE;
      }

      if (Orientation.NW == orientation) {
        return Orientation.E;
      }
      return orientation;
    }
  }
}
