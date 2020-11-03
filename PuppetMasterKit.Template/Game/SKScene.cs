using System.Linq;
using CoreGraphics;
using SpriteKit;
using LightInject;
using UIKit;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.AI;
using PuppetMasterKit.Utility.Extensions;
using PuppetMasterKit.Template.Game.Level;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.Graphics.Sprites;
using System.Collections.Generic;

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
    /// <param name="obstacles">Level.</param>
    public static void DrawObstacles(this SKScene scene, List<Obstacle> obstacles)
    {
      obstacles.OfType<CircularObstacle>().ForEach(c => DrawObstacle(scene, c));

      obstacles.OfType<PolygonalObstacle>().ForEach(c => DrawObstacle(scene, c));
    }

    /// <summary>
    /// Draws the obstacle.
    /// </summary>
    /// <param name="scene">Scene.</param>
    /// <param name="obstacle">Obstacle.</param>
    private static void DrawObstacle(SKScene scene, CircularObstacle obstacle)
    {
      var circle = SKShapeNode.FromCircle(obstacle.Radius);
      var mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      var center = mapper.ToScene(obstacle.Center);

      circle.Position = new CGPoint(center.X,center.Y);
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
      var mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      foreach (var _2dPoint in obstacle.Polygon.Points) {
        var point = mapper.ToScene(_2dPoint);
        if (prev == null) {
          path.MoveToPoint(point.X, point.Y);
        } else {
          path.AddLineToPoint(point.X, point.Y);
        }
        prev = point;
      }
      var lastPoint  = mapper.ToScene(new Point(obstacle.Polygon.Points[0].X, obstacle.Polygon.Points[0].Y));
      path.AddLineToPoint(new CGPoint(lastPoint.X, lastPoint.Y));

      var poly = SKShapeNode.FromPath(path);
      poly.FillColor = UIColor.Purple;
      poly.StrokeColor = UIColor.Purple;
      poly.LineWidth = 1;
      poly.ZPosition = 100;
      scene.AddChild(poly);

      obstacle.Polygon.Points.ForEach((x,index) => {
        var pos = mapper.ToScene(x);
        var circle = SKShapeNode.FromCircle(10);
        circle.Position = new CGPoint(pos.X, pos.Y);
        circle.FillColor = index == 0 ? UIColor.Red : (index == obstacle.Polygon.Count-1 ? UIColor.Blue : UIColor.Green);
        circle.ZPosition = 101;
        scene.AddChild(circle);
      });
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

    /// <summary>
    /// Draws the path.
    /// </summary>
    /// <param name="scene">Scene.</param>
    /// <param name="path">Path.</param>
    public static void DrawPath(this SKScene scene, List<Point> path)
    {
      if (path.Count == 0)
        return;
      
      var cgPath = new CGPath();
      var mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();

      var transformed = path.Select(mapper.ToScene);
      var first = transformed.First();
      cgPath.MoveToPoint(first.X, first.Y);
      transformed.Skip(1).ForEach(point => {
        cgPath.AddLineToPoint(point.X, point.Y);
      });
      var scenePath = SKShapeNode.FromPath(cgPath);
      scenePath.StrokeColor = UIColor.Yellow;
      scenePath.LineWidth = 2;
      scenePath.ZPosition = 100;
      scene.AddChild(scenePath);
    }
  }
}
