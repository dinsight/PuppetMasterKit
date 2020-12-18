using System;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using Foundation;
using PuppetMasterKit.Utility.Configuration;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game
{
  
  public class Hud : SKSpriteNode
  {
    public event EventHandler OnBuildingGranaryClick;

    public Hud(IntPtr handle) : base(handle)
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
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="touches"></param>
    /// <param name="evt"></param>
    public override void TouchesBegan(NSSet touches, UIEvent evt)
    {
      foreach (UITouch touch in touches) {  
        var positionInScene = touch.LocationInNode(this);
        var node = this.GetNodeAtPoint(positionInScene);
        if (node.Name == "build" && OnBuildingGranaryClick != null) {
          OnBuildingGranaryClick(this, null);
        }
      }
      base.TouchesBegan(touches, evt);
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
      var messageControl = this.Children.FirstOrDefault(x => x.Name == "message") as SKLabelNode;
      messageControl.Text = message;
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
