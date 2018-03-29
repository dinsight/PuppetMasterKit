using System.ComponentModel;
using LightInject;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.AI.Components
{
    public class SpriteComponent<T> : Component, IAgentDelegate 
    {
        ISprite theSprite;

        ITexture textureAtlas;

        public SpriteComponent(string atlasName, T initialState)
        {
            var factory = Container.GetContainer().GetInstance<ISpriteFactory>();
            textureAtlas = factory.CreateTexture(atlasName);
            theSprite = GetSprite(textureAtlas, Orientation.E, initialState);
            theSprite.AddToScene();
        }

        /// <summary>
        /// Gets the sprite.
        /// </summary>
        /// <returns>The sprite.</returns>
        /// <param name="texture">Texture.</param>
        /// <param name="orientation">Orientation.</param>
        /// <param name="state">State.</param>
        private ISprite GetSprite(ITexture texture, string orientation, 
                                  T state)
        {
            var suffix = $"-{state.ToString()}-{orientation}";
            return texture.GetSpriteWithSuffix(suffix);
        }

        /// <summary>
        /// Agent did update.
        /// </summary>
        /// <param name="agent">Agent.</param>
        public void AgentDidUpdate(Agent agent)
        {
            
        }

        /// <summary>
        /// Agent will update.
        /// </summary>
        /// <param name="agent">Agent.</param>
        public void AgentWillUpdate(Agent agent)
        {
            
        }

    }
}
