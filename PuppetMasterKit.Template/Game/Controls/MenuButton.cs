using System;
using Foundation;
using System.Linq;
using SpriteKit;
using UIKit;
using CoreGraphics;

namespace PuppetMasterKit.Template.Game.Controls
{
  public class MenuButton : HoverButton
  {
    public event EventHandler OnItemPressed;

    public event EventHandler OnItemReleased;

    private bool isPressed = false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    public MenuButton(IntPtr handle) : base(handle)
    {
      this.UserData = new Foundation.NSMutableDictionary();
      this.UserInteractionEnabled = true;
      SetupChildButtons();
      ShowMenu(false);
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetupChildButtons()
    {
      var padding = 5;
      for (int i = 0; i < this.Children.Count(); i++) {
        var item = this.Children[i] as HoverButton;
        if (item!=null) {
          var xpos = (i + 1) * (padding + item.Size.Width + HoverButton.Padding * 2);
          item.Position = new CGPoint(xpos, item.Position.Y);
          item.OnButtonPressed += Item_OnButtonPressed;
          item.OnButtonReleased += Item_OnButtonReleased;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Item_OnButtonPressed(object sender, EventArgs e)
    {
      OnItemPressed?.Invoke(sender, e);
      ShowMenu(false);
      SetNormalTexture();
      isPressed = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Item_OnButtonReleased(object sender, EventArgs e)
    {
      OnItemReleased?.Invoke(sender, e);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="touches"></param>
    /// <param name="evt"></param>
    public override void TouchesBegan(NSSet touches, UIEvent evt)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="touches"></param>
    /// <param name="evt"></param>
    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
      isPressed = !isPressed;
      if (isPressed) {
        SetHighlightedTexture();
        ShowMenu(true);
      } else {
        SetNormalTexture();
        ShowMenu(false);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="show"></param>
    private void ShowMenu(bool show)
    {
      var hide = SKAction.FadeOutWithDuration(1);
      var unhide = SKAction.FadeInWithDuration(1);
      this.Children.OfType<HoverButton>().ToList().ForEach(x => {
        if (show) {
          x.Hidden = false;
        } else {
          x.Hidden = true;
        }
      });
    }
  }
}
