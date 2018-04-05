using System;
using System.Collections.Generic;
using CoreGraphics;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Game.Ios.Bindings
{
  public class TextureFactory : ITextureFactory
  {
    SKScene scene;

    public TextureFactory(SKScene scene)
    {
      this.scene = scene;
    }

    /// <summary>
    /// Creates the texture.
    /// </summary>
    /// <returns>The texture.</returns>
    /// <param name="name">Name.</param>
    public ITexture CreateTexture(string name)
    {
      var texture = SKTextureAtlas.FromName(name);
      return new Texture(texture, scene);
    }
  }
}
