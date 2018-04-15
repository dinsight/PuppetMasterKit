using System;
namespace PuppetMasterKit.Graphics.Sprites
{
  public interface ISpriteFactory
  {
    ISprite CreateSprite(float r, float g, float b, float alpha);
  }
}
