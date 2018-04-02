using System;
using System.Collections.Generic;
using CoreGraphics;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Game.Ios.Bindings
{
    public class SpriteFactory : ISpriteFactory
    {
        SKScene scene;

        public SpriteFactory(SKScene scene)
        {
            this.scene = scene;
        }

        /// <summary>
        /// Creates the sprite.
        /// </summary>
        /// <returns>The sprite.</returns>
        /// <param name="red">Red.</param>
        /// <param name="green">Green.</param>
        /// <param name="blue">Blue.</param>
        /// <param name="alpha">Alpha.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public ISprite CreateSprite(float red, 
                                    float green, 
                                    float blue, 
                                    float alpha, 
                                    double width, 
                                    double height)
        {
            var color = new UIKit.UIColor(red, green, blue, alpha);
            var size = new CGSize(width, height);
            var sprite = SKSpriteNode.FromColor(color, size);
            sprite.UserData = new Foundation.NSMutableDictionary();
            return new Sprite(sprite, scene);
        }

        /// <summary>
        /// Creates the texture.
        /// </summary>
        /// <returns>The texture.</returns>
        /// <param name="name">Name.</param>
        public ITexture CreateTexture(string name)
        {
            var texture = SKTextureAtlas.FromName(name);
            return new Texture(texture, scene);
        }
    }
}
