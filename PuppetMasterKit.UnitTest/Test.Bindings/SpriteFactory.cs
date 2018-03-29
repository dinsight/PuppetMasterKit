using System;
using System.Collections.Generic;
using CoreGraphics;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Test.Bindings
{
    public class SpriteFactory : ISpriteFactory
    {
        public ISprite CreateSprite(float red, float green, float blue, float alpha, double width, double height)
        {
            return new Sprite();
        }

        public ITexture CreateTexture(string name)
        {
            return new Texture();
        }
    }
}
