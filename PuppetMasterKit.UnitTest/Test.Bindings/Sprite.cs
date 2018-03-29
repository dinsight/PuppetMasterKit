using System;
using System.Collections.Generic;
using Foundation;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Test.Bindings
{
    public class Sprite : ISprite
    {
        public float Alpha { get => 0; set {} }
        public Point Position { get => Point.Zero; set {} }
        public Point AnchorPoint { get => Point.Zero; set {} }

        public void AddChild(ISprite sprite)
        {
            
        }

        public void AddToScene()
        {
            
        }

        public void RemoveFromParent()
        {
            
        }
    }
}
