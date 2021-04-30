using System;
using CoreGraphics;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game.Controls
{
  public class ControlsUtil
  {
    private static readonly int Padding = 1;

    public static double GetAspectFillScaleFactor(SKScene scene)
    {
      var screenSize = UIScreen.MainScreen.Bounds.Size;
      var sceneSize = scene.Frame.Size;
      var factor = 1.0;
      if (screenSize.Height > sceneSize.Height) {
        factor = screenSize.Height / sceneSize.Height;
      } else if (screenSize.Width > sceneSize.Width) {
        factor = screenSize.Width / sceneSize.Width;
      }
      return factor;
    }

    public static CGSize GetVisibleScreenSize(SKScene scene)
    {
      var size = UIScreen.MainScreen.Bounds.Size;
      var factor = GetAspectFillScaleFactor(scene);
      return new CGSize(size.Width / factor - Padding, size.Height / factor - Padding);
    }
  }
}
