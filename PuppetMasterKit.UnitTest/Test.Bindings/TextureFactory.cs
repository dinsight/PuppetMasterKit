using System;
using System.Collections.Generic;
using CoreGraphics;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Test.Bindings
{
    public class TextureFactory : ITextureFactory
    {    
        public ITexture CreateTexture(string name)
        {
            return new Texture();
        }
    }
}
