using System;
using Foundation;
using SpriteKit;
using UIKit;
using System.Linq;
using PuppetMasterKit.Utility.Extensions;

namespace PuppetMasterKit.Template.Game.Controls.Buttons
{
  public class ToggleButton : CustomButton
  {
    private bool isPressed = false;

    public bool IsPressed { get => isPressed; }

    private static SKShader on = SKShader.FromFile("Shaders/Selected.fsh");
    private static SKShader off = SKShader.FromFile("Shaders/Neutral.fsh");


    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    public ToggleButton(IntPtr handle) : base(handle)
    {
      this.UserInteractionEnabled = true;
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
      //disable taps so they don't get confised with touches began
      this.Scene.View.GestureRecognizers
        .OfType<UITapGestureRecognizer>()
        .ForEach(x=>x.Enabled=false);

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
      this.Scene.View.GestureRecognizers
        .OfType<UITapGestureRecognizer>()
        .ForEach(x => x.Enabled = true);
    }

    protected void SetNormalTexture()
    {
      this.Shader = off;
    }

    protected void SetHighlightedTexture()
    {
      this.Shader = on;
    }
  }
}
