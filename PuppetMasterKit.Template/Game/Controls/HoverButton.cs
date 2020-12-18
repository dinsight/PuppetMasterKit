using System;
using Foundation;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game.Controls
{
  public class HoverButton : SKSpriteNode

  {
    SKTexture texture;

    public HoverButton(IntPtr handle) : base(handle)
    {
      this.UserInteractionEnabled = true;
    }

    public override void TouchesBegan(NSSet touches, UIEvent evt)
    {
      texture = this.Texture;
      this.Texture = SKTexture.FromImageNamed($"{this.Name}_hover");
      base.TouchesBegan(touches, evt);
    }

    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
      this.Texture = texture;
      base.TouchesEnded(touches, evt);
    }
  }
}
