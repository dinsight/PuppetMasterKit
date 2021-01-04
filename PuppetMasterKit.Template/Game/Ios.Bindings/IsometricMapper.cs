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

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Template.Game.Ios.Bindings.IsometricMapper"/> class.
    /// </summary>
    /// <param name="scene">Scene.</param>
    public IsometricMapper(SKScene scene)
    {
      this.scene = scene;
    }

    /// <summary>
    /// Froms the scene.
    /// </summary>
    /// <returns>The scene.</returns>
    /// <param name="point">Point.</param>
    public Point FromScene(Point point)
    {
      point = point * -1;
      return new Point(point.X + 2*point.Y, 2*point.Y - point.X);
    }

    /// <summary>
    /// Tos the scene.
    /// </summary>
    /// <returns>The scene.</returns>
    /// <param name="point">Point.</param>
    public Point ToScene(Point point)
    {
      return new Point((point.X - point.Y)/2, (point.X + point.Y)/4) * -1;
    }

    /// <summary>
    /// Tos the scene orientation.
    /// </summary>
    /// <returns>The scene orientation.</returns>
    /// <param name="orientation">Orientation.</param>
    public Orientation? ToSceneOrientation(Orientation? orientation)
    {
      if (!orientation.HasValue)
        return null;
      
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
