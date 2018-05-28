using System;
namespace PuppetMasterKit.Graphics.Sprites
{
  public interface ISpriteFactory
  {
    ISprite CreateSprite(float r, float g, float b, float alpha);
    ISprite FromTexture(String atlasName, double secondsPerFrame=1);
    ISprite ChangeTexture(ISprite sprite, String atlasName, double secondsPerFrame = 1);
  }
}
