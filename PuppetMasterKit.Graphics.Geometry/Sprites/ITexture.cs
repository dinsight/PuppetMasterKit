using System;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.Graphics.Sprites
{
    public interface ITexture
    {
        ISprite GetSprite(string name);
        ISprite GetSpriteWithSuffix(string suffix);
    }
}
