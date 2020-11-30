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

    public ISprite FromTexture(String atlasName, double secondsPerFrame)
    {
      return new Sprite();
    }

    public ISprite FromTexture(String atlasName, String imageName)
    {
      return new Sprite();
    }

    public ISprite ChangeTexture(ISprite sprite, String atlasName, double secondsPerFrame = 1)
    {
      return sprite;
    }
  }
}
