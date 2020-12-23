using System;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game.Controls
{
  public class HoverButton : SKSpriteNode
  {
    public readonly static int Padding = 5;

    public event EventHandler OnButtonPressed;
    public event EventHandler OnButtonReleased;
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

      selection = new SKSpriteNode();
      selection.Texture = neutralTexture;
      selection.AnchorPoint = new CGPoint(0.5, 0.5);
      selection.Position = new CGPoint(0, 0);
      selection.Size = new CGSize(this.Size.Width + Padding, this.Size.Height + Padding);
      selection.UserInteractionEnabled = false;
      selection.ZPosition = -1;
      this.AddChild(selection);
      this.UserInteractionEnabled = true;
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
      OnButtonPressed?.Invoke(this, null);
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
      OnButtonReleased?.Invoke(this, null);
    }

    /// <summary>
    /// 
    /// </summary>
    protected void SetNormalTexture() {
      selection.Texture = neutralTexture;
    }

    /// <summary>
    /// 
    /// </summary>
    protected void SetHighlightedTexture() {
      selection.Texture = selectedTexture;
    }
  }
}
