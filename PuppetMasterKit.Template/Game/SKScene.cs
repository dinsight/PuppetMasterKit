using System;
using CoreGraphics;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game
{
  public static class SKSceneExtension
  {
    public static CGRect GetFrame(this SKScene scene)
    {
      var frame = scene.Frame;
      var orientation = UIDevice.CurrentDevice.Orientation;
      if(orientation == UIDeviceOrientation.LandscapeLeft || 
         orientation == UIDeviceOrientation.LandscapeRight){
        return new CGRect(0, 0, frame.Height, frame.Width);
      }
      return frame;
    }
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
  }
}
