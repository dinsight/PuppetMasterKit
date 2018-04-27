using System;
using System.Reflection;
using CoreGraphics;
using SpriteKit;
using UIKit;
using Newtonsoft.Json;
using System.IO;
using PuppetMasterKit.Graphics.Geometry;

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
      foreach (var item in level.Obstacles) {
        Point prev = null;

        var path = new CGPath();
        foreach (var point in item.Points) {

          if(prev==null){
            path.MoveToPoint(point.X, point.Y);
          } else {
            path.AddLineToPoint(point.X, point.Y);
          }
          prev = point;
        }
        path.AddLineToPoint(item.Points[0].X, item.Points[0].Y);

        var poly = SKShapeNode.FromPath(path);
        poly.FillColor = UIColor.Yellow;
        poly.StrokeColor = UIColor.Yellow;
        poly.LineWidth = 1;
        poly.ZPosition = 100;
        scene.AddChild(poly);
      }
    }
  }
}
