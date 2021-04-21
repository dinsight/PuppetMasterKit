using System;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game.Controls.Buttons
{
  public class HoverButton : CustomButton
  {
    public readonly static int Padding = 5;
    private SKShader on = SKShader.FromFile("Shaders/Wind.fsh");

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    public HoverButton(IntPtr handle) : base(handle)
    {
      
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

    public override void TouchesCancelled(NSSet touches, UIEvent evt)
    {
      base.TouchesCancelled(touches, evt);
      SetNormalTexture();
      NotifyButtonReleased(this, null);
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void SetNormalTexture() {
      this.Shader = null;
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void SetHighlightedTexture() {
      this.Shader = on;
    }
  }
}
