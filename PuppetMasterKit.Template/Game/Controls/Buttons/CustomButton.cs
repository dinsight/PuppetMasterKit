using System;
using System.Runtime.CompilerServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using SpriteKit;

namespace PuppetMasterKit.Template.Game.Controls.Buttons
{
  public class CustomButton : SKSpriteNode
  {
    public event EventHandler   OnButtonPressed;
    public event EventHandler OnButtonReleased;
    NSMutableDictionary _dict;
      
    public CustomButton(IntPtr handle) : base(handle)
    {
    }

    protected void NotifyButtonPressed(object sender, EventArgs args) {
      OnButtonPressed?.Invoke(sender, args);
    }

    protected void NotifyButtonReleased(object sender, EventArgs args)
    {
      OnButtonReleased?.Invoke(sender, args);
    }

    public override NSMutableDictionary UserData {
      [Export("userData", ArgumentSemantic.Copy)]
      get => _dict;
      [Export("setUserData:", ArgumentSemantic.Copy)]
      set => _dict = value;
    }

    public virtual void UpdateLayout() {
    }
  }
}
