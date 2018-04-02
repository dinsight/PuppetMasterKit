using System;
using LightInject;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;

namespace PuppetMasterKit.AI.Components
{
  public class SpriteComponent : Component, IAgentDelegate
  {
    ISprite theSprite;

    ITexture textureAtlas;

    int orientationCounter = 0;

    int smoothOrientationThreshold = 3;

    string currentOrientation = null;

    /// <summary>
    /// Gets the sprite.
    /// </summary>
    /// <value>The sprite.</value>
    public ISprite Sprite {
      get {
        return theSprite;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Components.SpriteComponent"/> class.
    /// </summary>
    /// <param name="atlasName">Atlas name.</param>
    public SpriteComponent(string atlasName, Size size = null)
    {
      var factory = Container.GetContainer().GetInstance<ISpriteFactory>();
      textureAtlas = factory.CreateTexture(atlasName);
      theSprite = GetSprite(textureAtlas, Orientation.E, null);
      if (theSprite != null) {
        theSprite.AddToScene();
        if (size != null) {
          theSprite.Size = size;
        }
      }
    }

    /// <summary>
    /// Gets the sprite.
    /// </summary>
    /// <returns>The sprite.</returns>
    /// <param name="texture">Texture.</param>
    /// <param name="orientation">Orientation.</param>
    /// <param name="state">State.</param>
    private ISprite GetSprite(ITexture texture, string orientation,
                              string state)
    {
      var name = state == null ? String.Empty : $"-{state}";
      var suffix = $"{name}-{orientation}";
      return texture.GetSpriteWithSuffix(suffix);
    }

    /// <summary>
    /// Agent will update.
    /// </summary>
    /// <param name="agent">Agent.</param>
    public void AgentWillUpdate(Agent agent)
    {
      agent.Position = theSprite.Position;
    }

    /// <summary>
    /// Agent did update.
    /// </summary>
    /// <param name="agent">Agent.</param>
    public void AgentDidUpdate(Agent agent)
    {
      if (theSprite.Position != null) {
        var direction = agent.Position - theSprite.Position;
        var orientation = Orientation.GetOrientation(direction);

        orientationCounter++;
        if (orientation != currentOrientation) {
          currentOrientation = orientation;
          orientationCounter = 0;
        }

        if (orientationCounter > smoothOrientationThreshold) {
          var state = Entity.GetComponent<StateComponent>()?.ToString();
          if (state != null) {
            var newTexture = GetSprite(textureAtlas, orientation, state);
            if (newTexture != null) {
              theSprite = newTexture;
            }
          }
        }
      }
      theSprite.Position = agent.Position;
    }
  }
}
