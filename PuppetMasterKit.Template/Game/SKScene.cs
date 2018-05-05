using System.Linq;
using CoreGraphics;
using SpriteKit;
using UIKit;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.AI;
using PuppetMasterKit.Utility;
using PuppetMasterKit.Template.Game.Level;

namespace PuppetMasterKit.Template.Game
{
  public static class SKSceneExtension
  {
    /// <summary>
    /// Gets the frame.
    /// </summary>
    /// <returns>The frame.</returns>
    /// <param name="scene">Scene.</param>
    public static CGRect GetFrame(this SKScene scene)
    {
      var frame = scene.Frame;
      var orientation = UIDevice.CurrentDevice.Orientation;
      if (orientation == UIDeviceOrientation.LandscapeLeft ||
         orientation == UIDeviceOrientation.LandscapeRight) {
        return new CGRect(0, 0, frame.Height, frame.Width);
      }
      return frame;
    }

    /// <summary>
    /// Gets the view frame.
    /// </summary>
    /// <returns>The view frame.</returns>
    /// <param name="scene">Scene.</param>
    public static CGRect GetViewFrame(this SKScene scene)
    {
      var frame = scene.View.Frame;
      var orientation = UIDevice.CurrentDevice.Orientation;
      if (orientation == UIDeviceOrientation.LandscapeLeft ||
         orientation == UIDeviceOrientation.LandscapeRight) {
        return new CGRect(0, 0, frame.Height, frame.Width);
      }
      return frame;
    }

    /// <summary>
    /// Draws the obstacles.
    /// </summary>
    /// <param name="scene">Scene.</param>
    /// <param name="level">Level.</param>
    public static void DrawObstacles(this SKScene scene, LevelData level)
    {
      level.Obstacles.OfType<CircularObstacle>().ForEach(c => DrawObstacle(scene, c));

      level.Obstacles.OfType<PolygonalObstacle>().ForEach(c => DrawObstacle(scene, c));
    }

    /// <summary>
    /// Draws the obstacle.
    /// </summary>
    /// <param name="scene">Scene.</param>
    /// <param name="obstacle">Obstacle.</param>
    private static void DrawObstacle(SKScene scene, CircularObstacle obstacle)
    {
      var circle = SKShapeNode.FromCircle(obstacle.Radius);

      circle.Position = new CGPoint(obstacle.Center.X,obstacle.Center.Y);
      //circle.FillColor = UIColor.Yellow;
      circle.StrokeColor = UIColor.Yellow;
      circle.LineWidth = 1;
      circle.ZPosition = 100;
      scene.AddChild(circle);
    }

    /// <summary>
    /// Draws the obstacle.
    /// </summary>
    /// <param name="scene">Scene.</param>
    /// <param name="obstacle">Obstacle.</param>
    private static void DrawObstacle(SKScene scene, PolygonalObstacle obstacle)
    {
      Point prev = null;

      var path = new CGPath();
      foreach (var point in obstacle.Polygon.Points) {

        if (prev == null) {
          path.MoveToPoint(point.X, point.Y);
        } else {
          path.AddLineToPoint(point.X, point.Y);
        }
        prev = point;
      }
      path.AddLineToPoint(obstacle.Polygon.Points[0].X, obstacle.Polygon.Points[0].Y);

      var poly = SKShapeNode.FromPath(path);
      poly.FillColor = UIColor.Yellow;
      poly.StrokeColor = UIColor.Yellow;
      poly.LineWidth = 1;
      poly.ZPosition = 100;
      scene.AddChild(poly);
    }

    /// <summary>
    /// Draws the enclosure.
    /// </summary>
    /// <param name="scene">Scene.</param>
    public static void DrawEnclosure(this SKScene scene)
    {
      var path = new CGPath();
      var frame = GetFrame(scene);

      path.AddRect(frame);
      var poly = SKShapeNode.FromPath(path);
      poly.StrokeColor = UIColor.Red;
      poly.LineWidth = 1;
      poly.ZPosition = 100;
      scene.AddChild(poly);
    }
  }
}
