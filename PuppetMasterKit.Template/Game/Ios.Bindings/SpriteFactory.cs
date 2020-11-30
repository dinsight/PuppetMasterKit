using System;
using System.Linq;
using System.Collections.Generic;
using CoreGraphics;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game.Ios.Bindings
{
  public class SpriteFactory : ISpriteFactory
  {
    private SKScene scene;

    private static string animationKey = "animateKey";

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

    /// <summary>
    /// Froms the texture.
    /// </summary>
    /// <returns>The texture.</returns>
    /// <param name="atlasName">Atlas name.</param>
    public ISprite FromTexture(String atlasName, double secondsPerFrame)
    {
      var node = new SKSpriteNode();
      node.UserData = new Foundation.NSMutableDictionary();
      node.PhysicsBody = null;
      var sprite = new Sprite(node, scene);
      return ChangeTexture(sprite, atlasName, secondsPerFrame);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="atlasName"></param>
    /// <param name="imageName"></param>
    /// <returns></returns>
    public ISprite FromTexture(String atlasName, String imageName) {
      var node = new SKSpriteNode();
      node.UserData = new Foundation.NSMutableDictionary();
      node.PhysicsBody = null;
      var sprite = new Sprite(node, scene);
      var atlas = SKTextureAtlas.FromName(atlasName);
      node.RemoveActionForKey(animationKey);
      node.Texture = atlas.TextureNamed(imageName);
      return sprite;
    }

    /// <summary>
    /// Changes the texture.
    /// </summary>
    /// <returns>The texture.</returns>
    /// <param name="sprite">Sprite.</param>
    /// <param name="atlasName">Atlas name.</param>
    /// <param name="secondsPerFrame">Seconds per frame.</param>
    public ISprite ChangeTexture(ISprite sprite, String atlasName, double secondsPerFrame)
    {
      var atlas = SKTextureAtlas.FromName(atlasName);
      var names = atlas.TextureNames;
      if (names.Length == 0)
        return null;

      var first = atlas.TextureNamed(names[0]);
      var node = sprite.GetNativeSprite() as SKSpriteNode;
      node.RemoveActionForKey(animationKey);
      node.Texture = first;

      //if we have more than one texture, add an animation action
      if (names.Length > 1) {
        List<SKTexture> textures = new List<SKTexture>();
        var sorted = atlas.TextureNames.OrderBy(x => x);
        foreach (var item in sorted) {
          textures.Add(atlas.TextureNamed(item));
        }
        node.RunAction(SKAction.RepeatActionForever(
          SKAction.AnimateWithTextures(
            textures.ToArray(), 
            secondsPerFrame, false, true)), animationKey);
      }

      return sprite;
    }
  }
}
