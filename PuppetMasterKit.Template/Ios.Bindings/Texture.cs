using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Ios.Bindings
{
    public class Texture : ITexture
    {
        private SKTextureAtlas atlas;

        private SKScene scene;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Template.Ios.Bindings.Texture"/> class.
        /// </summary>
        /// <param name="atlas">Atlas.</param>
        public Texture(SKTextureAtlas atlas, SKScene scene)
        {
            this.atlas = atlas;
            this.scene = scene;
        }

        /// <summary>
        /// Gets the sprite.
        /// </summary>
        /// <returns>The sprite.</returns>
        /// <param name="name">Name.</param>
        public ISprite GetSprite(string name){
            var texture = atlas.TextureNamed(name);
            var node = new SKSpriteNode(texture);
            node.UserData = new Foundation.NSMutableDictionary();
            return new Sprite(node, scene);
        }

        /// <summary>
        /// Gets the sprite with suffix.
        /// </summary>
        /// <returns>The sprite with suffix.</returns>
        /// <param name="suffix">Suffix.</param>
        public ISprite GetSpriteWithSuffix(string suffix)
        {
            var textureName = 
                atlas.TextureNames.FirstOrDefault(x => 
                    x.EndsWith(suffix, System.StringComparison.CurrentCulture));

            if (textureName!=null) {
                var node = new SKSpriteNode(textureName);
                node.UserData = new Foundation.NSMutableDictionary();
                return new Sprite(node, scene);
            }
            return null;
        }
    }
}
