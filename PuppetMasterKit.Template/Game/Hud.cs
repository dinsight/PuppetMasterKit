using System;
using System.Linq;
using CoreGraphics;
using SpriteKit;

namespace PuppetMasterKit.Template.Game
{
  public class Hud
  {
    public SKSpriteNode Menu { get; private set; }

    private Hud(){}

    /// <summary>
    /// Create the specified fromFile, contolName and inScene.
    /// </summary>
    /// <returns>The create.</returns>
    /// <param name="fromFile">From file.</param>
    /// <param name="controlName">Contol name.</param>
    public static Hud Create(string fromFile, string controlName)
    {
      var hud = new Hud();
      var scn = SKNode.FromFile<SKScene>(fromFile);
      hud.Menu = scn.Children.FirstOrDefault(x => x.Name == controlName) as SKSpriteNode;
      hud.Menu.RemoveFromParent();
      return hud;
    }

    /// <summary>
    /// Updates the score.
    /// </summary>
    /// <param name="score">Score.</param>
    public void UpdateScore(int score)
    {
      var scoreControl = Menu.Children.FirstOrDefault(x => x.Name == "score") as SKLabelNode;
      scoreControl.Text = $"{score}";
    }

    /// <summary>
    /// Sets the message.
    /// </summary>
    /// <param name="message">Message.</param>
    public void SetMessage(string message)
    {
      var messageControl = Menu.Children.FirstOrDefault(x => x.Name == "message") as SKLabelNode;
      messageControl.Text = message;
    }

    /// <summary>
    /// Updates the health.
    /// </summary>
    /// <param name="maxHealth">Max health.</param>
    /// <param name="damage">Damage.</param>
    public void UpdateHealth(int maxHealth, int damage)
    {
      var healthBar = Menu.Children.FirstOrDefault(x => x.Name == "health") as SKSpriteNode;
      var damageBar = Menu.Children.FirstOrDefault(x => x.Name == "damage") as SKSpriteNode;
      damageBar.Size = new CGSize( damage * healthBar.Size.Width / maxHealth, damageBar.Size.Height);
    }
  }
}
