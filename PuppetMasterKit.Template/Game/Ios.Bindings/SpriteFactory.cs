using System;
using CoreGraphics;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game.Ios.Bindings
{
  public class SpriteFactory : ISpriteFactory
  {
    private SKScene scene;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Template.Game.Ios.Bindings.SpriteFactory"/> class.
    /// </summary>
    /// <param name="scene">Scene.</param>
    public SpriteFactory(SKScene scene)
    {
      this.scene = scene;
    }

    /// <summary>
    /// Creates the sprite.
    /// </summary>
    /// <returns>The sprite.</returns>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="alpha">Alpha.</param>
    public ISprite CreateSprite(float r, float g, float b, float alpha)
    {
      var node = new SKSpriteNode(UIColor.FromRGB(r,g,b), new CGSize(0,0));
      node.Alpha = alpha;
      node.UserData = new Foundation.NSMutableDictionary();
      node.PhysicsBody = null;
      return new Sprite(node, scene);
    }
  }
}
