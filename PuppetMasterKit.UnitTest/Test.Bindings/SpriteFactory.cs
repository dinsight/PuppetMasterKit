using System;
using PuppetMasterKit.Graphics.Sprites;

namespace PuppetMasterKit.Template.Test.Bindings
{
  public class SpriteFactory : ISpriteFactory
  {
    public SpriteFactory()
    {
    }

    public ISprite CreateSprite(float r, float g, float b, float alpha)
    {
      return new Sprite();
    }
  }
}
