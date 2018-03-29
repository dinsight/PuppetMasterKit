﻿using System;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Ios.Bindings
{
    public static class Registration
    {
        public static void RegisterBindings(SKScene scene)
        {
            Container.GetContainer().Register<ISpriteFactory>(factory => new SpriteFactory(scene));
        }
    }
}
