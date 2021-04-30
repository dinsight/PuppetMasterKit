using System;
using Foundation;
using System.Linq;
using SpriteKit;
using UIKit;
using CoreGraphics;

namespace PuppetMasterKit.Template.Game.Controls.Buttons
{
  public class MenuButton : HoverButton
  {
    private bool isPressed = false;

    private int itemSpacing = 0;

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
      for (int i = 0; i < this.Children.Count(); i++) {
        var item = this.Children[i] as HoverButton;
        if (item!=null) {
          var xpos = (i + 1) * (itemSpacing + item.Size.Width + HoverButton.Padding * 2);
          item.Position = new CGPoint(0, 0);
          item.OnButtonPressed += Item_OnButtonPressed;
          item.OnButtonReleased += Item_OnButtonReleased;
          item.Alpha = 0;
          
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
      NotifyButtonPressed(sender, e);
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
      NotifyButtonReleased(sender, e);
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
      var reveal = SKAction.FadeInWithDuration(1);
      var list = this.Children.OfType<HoverButton>().ToList();
      for (int i = 0; i < list.Count; i++) {
        var x = list[i];
        if (show) {
          x.Hidden = false;
          var xpos = (i + 1) * (itemSpacing + x.Size.Width + HoverButton.Padding * 2);
          var slide = SKAction.MoveTo(new CGPoint(xpos,0), 0.5);
          var group = SKAction.Group( reveal, slide);
          x.RunAction(group); 
        } else {
          var slide = SKAction.MoveTo(new CGPoint(0,0), 0.5);
          var group = SKAction.Group(slide, hide, SKAction.Run(() => x.Hidden = true));
          x.RunAction(group);
        }
      }
    }
  }
}
