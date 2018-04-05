﻿using System;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Game.Ios.Bindings
{
    public static class Registration
    {
        public static void RegisterBindings(SKScene scene)
        {
            Container.GetContainer().Register<ITextureFactory>(factory => new TextureFactory(scene));
        }
    }
}
