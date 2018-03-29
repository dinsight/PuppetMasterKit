using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Test.Bindings
{
    public class Texture : ITexture
    {
        public ISprite GetSprite(string name)
        {
            return new Sprite();
        }

        public ISprite GetSpriteWithSuffix(string suffix)
        {
            return new Sprite();
        }
    }
}
