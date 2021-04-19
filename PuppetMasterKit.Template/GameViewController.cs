using System;
using PuppetMasterKit.Template.Game;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template
{
  public partial class GameViewController : UIViewController, IUIGestureRecognizerDelegate
  {
    protected GameViewController(IntPtr handle) : base(handle)
    {
      // Note: this .ctor should not contain any initialization logic.
    }

    public override void ViewDidLoad()
    {
      base.ViewDidLoad();

      // Configure the view.
      var skView = (SKView)View;
      skView.ShowsFPS = true;
      skView.ShowsNodeCount = true;
      /* Sprite Kit applies additional optimizations to improve rendering performance */
      skView.IgnoresSiblingOrder = false;


      // Create and configure the scene.
      var scene = SKNode.FromFile<GameScene>("GameScene");
      scene.ScaleMode = SKSceneScaleMode.AspectFill;

      // Present the scene.
      skView.PresentScene(scene);
    }

    public override bool ShouldAutorotate()
    {
      return true;
    }

    public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
    {
      return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? UIInterfaceOrientationMask.AllButUpsideDown : UIInterfaceOrientationMask.All;
    }

    public override void DidReceiveMemoryWarning()
    {
      base.DidReceiveMemoryWarning();
      // Release any cached data, images, etc that aren't in use.
    }

    public override bool PrefersStatusBarHidden()
    {
      return true;
    }
  }
}
