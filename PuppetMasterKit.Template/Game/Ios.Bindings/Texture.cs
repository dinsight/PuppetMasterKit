﻿using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Game.Ios.Bindings
{
  public class Texture : ITexture
  {
    private SKTextureAtlas atlas;

    private SKScene scene;

    private SKSpriteNode node;

    private Sprite sprite;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Template.Ios.Bindings.Texture"/> class.
    /// </summary>
    /// <param name="atlas">Atlas.</param>
    public Texture(SKTextureAtlas atlas, SKScene scene)
    {
      this.atlas = atlas;
      this.scene = scene;
      this.node = new SKSpriteNode();
      node.UserData = new Foundation.NSMutableDictionary();
      node.PhysicsBody = null;
      sprite = new Sprite(node, scene);
    }

    /// <summary>
    /// Gets the sprite.
    /// </summary>
    /// <returns>The sprite.</returns>
    /// <param name="name">Name.</param>
    public ISprite GetSprite(string name)
    {
      var texture = atlas.TextureNamed(name);
      node.Texture = texture;
      return sprite;
    }

    /// <summary>
    /// Gets the sprite with suffix.
    /// </summary>
    /// <returns>The sprite with suffix.</returns>
    /// <param name="suffix">Suffix.</param>
    public ISprite GetSpriteWithSuffix(string suffix)
    {
      var textureName =
          atlas.TextureNames.FirstOrDefault(x =>
              x.EndsWith(suffix, System.StringComparison.CurrentCulture));

      if (textureName != null) {
        var texture = atlas.TextureNamed(textureName);
        node.Texture = texture;
        return sprite;
      }
      return null;
    }
  }
}
