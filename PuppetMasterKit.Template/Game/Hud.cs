﻿using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using PuppetMasterKit.Template.Game.Controls;
using PuppetMasterKit.Template.Game.Controls.Buttons;
using PuppetMasterKit.Utility.Configuration;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game
{

  public class Hud : SKSpriteNode
  {
    private const string HELP_BUTTON = "help";

    public event EventHandler<String> OnHudButtonClick;

    public event EventHandler<UISwipeGestureRecognizer> OnShowMenu;

    private UISwipeGestureRecognizer onShowMenuGesture;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    public Hud(IntPtr handle) : base(handle)
    {
      WireUpButtons();

      onShowMenuGesture = new UISwipeGestureRecognizer(gesture=>
          OnShowMenu?.Invoke(this, gesture)) {
        Direction = UISwipeGestureRecognizerDirection.Down
      };
    }

    /// <summary>
    /// Create the specified fromFile, contolName and inScene.
    /// </summary>
    /// <returns>The create.</returns>
    /// <param name="fromFile">From file.</param>
    /// <param name="controlName">Contol name.</param>
    public static Hud Create(string fromFile, string controlName)
    {
      var scn = SKNode.FromFile<SKScene>(fromFile);
      var hud = scn.Children.FirstOrDefault(x => x.Name == controlName) as Hud;
      hud.RemoveFromParent();
      hud.UserInteractionEnabled = true;
      Container.GetContainer().RegisterInstance<Hud>(hud);
      return hud;
    }

    /// <summary>
    /// 
    /// </summary>
    private void WireUpButtons()
    {
      foreach (var item in this.Children.OfType<HoverButton>()) {
        item.OnButtonPressed += Item_OnButtonPressed;
        item.OnButtonReleased += Item_OnButtonReleased;
      }
      foreach (var item in this.Children.OfType<MenuButton>()) {
        item.OnButtonPressed += Item_OnButtonPressed;
      }

      var helpNode = this.Children.FirstOrDefault(x => x.Name == HELP_BUTTON) as ToggleButton;
      if (helpNode != null) {
        helpNode.OnButtonPressed += HelpNode_OnButtonPressed;
        helpNode.OnButtonReleased += HelpNode_OnButtonReleased;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HelpNode_OnButtonReleased(object sender, EventArgs e)
    {
      SetMessage(String.Empty);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HelpNode_OnButtonPressed(object sender, EventArgs e)
    {
      SetMessage(
          "- <Swipe Down> to see the selected character's menu\n" +
          "- <Tap> to move the selected character\n" +
          "- <Double Tap> to attack location ");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Item_OnButtonPressed(object sender, EventArgs e)
    {
      var button = sender as HoverButton;
      OnHudButtonClick?.Invoke(sender, button.Name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Item_OnButtonReleased(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="cameraNode"></param>
    public void UpdateLayout(SKScene scene, SKCameraNode cameraNode) {
      var sz = new CGPoint(scene.View.Bounds.Width, scene.View.Bounds.Height);
      var s = scene.ConvertPointFromView(sz);
      var t = cameraNode.ConvertPointFromNode(s, scene);
      this.Position = new CGPoint(-Math.Abs(t.X), -Math.Abs(t.Y));

      var scoreControl = this.Children.FirstOrDefault(x => x.Name == "score") as SKLabelNode;
      var messageControl = this.Children.FirstOrDefault(x => x.Name == "message") as SKLabelNode;
      var helpNode = this.Children.FirstOrDefault(x => x.Name == HELP_BUTTON) as ToggleButton;

      var isLandscape = UIApplication.SharedApplication.StatusBarOrientation.IsLandscape();
      const int msgMargin = 35;
      int topMargin = isLandscape ? 5: 30;
      var visibleSize = ControlsUtil.GetVisibleScreenSize(scene);
      
      messageControl.PreferredMaxLayoutWidth = visibleSize.Width - 1f * msgMargin;
      messageControl.Position = new CGPoint(visibleSize.Width/2, visibleSize.Height - topMargin);

      foreach (var item in this.Children.OfType<CustomButton>()) {
        item.UpdateLayout();
      }

      var marginTop = isLandscape ? 10 : 50;
      helpNode.Position = new CGPoint(
        visibleSize.Width,
        visibleSize.Height - marginTop);

      scene.View.AddGestureRecognizer(onShowMenuGesture);
    }

    /// <summary>
    /// Updates the score.
    /// </summary>
    /// <param name="score">Score.</param>
    public void UpdateScore(int score)
    {
      var scoreControl = this.Children.FirstOrDefault(x => x.Name == "score") as SKLabelNode;
      scoreControl.Text = $"{score}";
    }

    /// <summary>
    /// Sets the message.
    /// </summary>
    /// <param name="message">Message.</param>
    public void SetMessage(string message)
    {
      if (String.IsNullOrEmpty(message))
        message = " ";
      var messageControl = this.Children.FirstOrDefault(x => x.Name == "message") as SKLabelNode;

      var mutable = new NSMutableAttributedString(messageControl.AttributedText);
      mutable.MutableString.SetString(new NSString(message));
      messageControl.AttributedText = mutable;
    }

    /// <summary>
    /// Updates the health.
    /// </summary>
    /// <param name="maxHealth">Max health.</param>
    /// <param name="damage">Damage.</param>
    public void UpdateHealth(int maxHealth, int damage)
    {
      var healthBar = this.Children.FirstOrDefault(x => x.Name == "health") as SKSpriteNode;
      var damageBar = this.Children.FirstOrDefault(x => x.Name == "damage") as SKSpriteNode;
      damageBar.Size = new CGSize( damage * healthBar.Size.Width / maxHealth, damageBar.Size.Height);
    }
  }
}
