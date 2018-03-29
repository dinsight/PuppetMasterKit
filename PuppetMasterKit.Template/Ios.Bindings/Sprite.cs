using System;
using System.Collections.Generic;
using Foundation;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Ios.Bindings
{
    public class Sprite : ISprite
    {
        SKScene scene;
        SKSpriteNode node;

        public Sprite(SKSpriteNode node, SKScene scene)
        {
            this.scene = scene;
            this.node = node;
        }

        public float Alpha { 
            get
            {
                return (float)node.Alpha;
            }
            set
            {
                node.Alpha = value;
            }
        }

        public Point Position { 
            get
            {
                return 
                    new Point( (float)node.Position.X, (float)node.Position.Y);
            }

            set {
                node.Position = new CoreGraphics.CGPoint(value.X, value.Y);
            }
        }

        public Point AnchorPoint { 
            get
            {
                return
                    new Point((float)node.AnchorPoint.X, (float)node.AnchorPoint.Y);
            }

            set
            {
                node.AnchorPoint = new CoreGraphics.CGPoint(value.X, value.Y);
            }
        }

        public void AddChild(ISprite sprite)
        {
            var local = sprite as Sprite;
            node.AddChild(local.node);
        }

        public void AddToScene()
        {
            scene.AddChild(node);
        }

        public void RemoveFromParent()
        {
            node.RemoveFromParent();
        }
    }
}
