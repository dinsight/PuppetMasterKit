using System;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game.Controls
{
  public class HoverButton : CustomButton
  {
    public readonly static int Padding = 5;
    protected SKTexture selectedTexture;
    protected SKTexture neutralTexture;
    private SKSpriteNode selection;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    public HoverButton(IntPtr handle) : base(handle)
    {
      selectedTexture = SKTexture.FromImageNamed("selected");
      neutralTexture = SKTexture.FromImageNamed("neutral");

      selection = new SKSpriteNode {
        Texture = neutralTexture,
        AnchorPoint = new CGPoint(0.5, 0.5),
        Position = new CGPoint(0, 0),
        UserInteractionEnabled = false,
        ZPosition = this.ZPosition-1
      };
      this.AddChild(selection);
      this.UserInteractionEnabled = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newSize"></param>
    public override void UpdateLayout()
    {
      selection.Size = new CGSize(this.Size.Width + Padding, this.Size.Height + Padding);
      base.UpdateLayout();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="touches"></param>
    /// <param name="evt"></param>
    public override void TouchesBegan(NSSet touches, UIEvent evt)
    {
      base.TouchesBegan(touches, evt);
      SetHighlightedTexture();
      NotifyButtonPressed(this, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="touches"></param>
    /// <param name="evt"></param>
    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
      base.TouchesEnded(touches, evt);
      SetNormalTexture();
      NotifyButtonReleased(this, null);
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void SetNormalTexture() {
      selection.Texture = neutralTexture;
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void SetHighlightedTexture() {
      selection.Texture = selectedTexture;
    }
  }
}
