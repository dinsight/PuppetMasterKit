using System;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Test.Bindings
{
    public static class Registration
    {
        public static void RegisterBindings()
        {
            Container.GetContainer().Register<ITextureFactory>(factory => new TextureFactory());
        }
    }
}
