using System;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.Graphics.Sprites
{
    public interface ISpriteFactory
    {
        ITexture CreateTexture(string name);

        ISprite CreateSprite(float red, float green, float blue, float alpha,
                             double width, double height);
    }
}
