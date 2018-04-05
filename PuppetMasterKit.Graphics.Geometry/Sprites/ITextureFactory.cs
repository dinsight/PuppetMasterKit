using System;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.Graphics.Sprites
{
    public interface ITextureFactory
    {
        ITexture CreateTexture(string name);
    }
}
