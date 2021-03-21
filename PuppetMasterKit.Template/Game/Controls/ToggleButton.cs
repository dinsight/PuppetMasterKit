using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game.Controls
{
  public class ToggleButton : HoverButton
  {
    private bool isPressed = false;

    public bool IsPressed { get => isPressed; }

    SKShader on = SKShader.FromFile("Shaders/Selected.fsh");

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    public ToggleButton(IntPtr handle) : base(handle)
    {
      //selectedTexture = SKTexture.FromImageNamed("selected");
      //neutralTexture = SKTexture.FromImageNamed("neutral");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isButtonPressed"></param>
    public void ToggleState(bool isButtonPressed) {
      if (isButtonPressed) {
        SetHighlightedTexture();
      } else {
        SetNormalTexture();
      }
      isPressed = isButtonPressed;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="touches"></param>
    /// <param name="evt"></param>
    public override void TouchesBegan(NSSet touches, UIEvent evt)
    {
      isPressed = !isPressed;
      ToggleState(isPressed);
      if (isPressed) {
        NotifyButtonPressed(this, null);
      } else {
        NotifyButtonReleased(this, null);
      }
    }

    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
      
    }

    protected override void SetNormalTexture()
    {
      this.Shader = null;
    }

    protected override void SetHighlightedTexture()
    {
      this.Shader = on;
    }
  }
}
