using System;
using LightInject;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;

namespace PuppetMasterKit.AI.Components
{
  public class SpriteComponent : Component, IAgentDelegate
  {
    private ISprite theSprite;

    private ITexture textureAtlas;

    private int orientationCounter = 0;

    private int smoothOrientationThreshold = 3;

    private string currentOrientation = null;

    public const string  ENTITY_ID_PPROPERTY = "id";

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
      var factory = Container.GetContainer().GetInstance<ITextureFactory>();
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
    /// On set entity
    /// </summary>
    public override void OnSetEntity()
    {
      theSprite.AddProperty(ENTITY_ID_PPROPERTY, Entity.Id);
      base.OnSetEntity();
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
          var state = Entity.GetComponent<StateComponent>();
          if (state != null) {
            var newTexture = GetSprite(textureAtlas, orientation, state.ToString());
            if (newTexture != null) {
              theSprite = newTexture;
            }
            SetSelection(state);
          }
        }
      }
      theSprite.Position = agent.Position;
    }

    /// <summary>
    /// Sets the selection.
    /// </summary>
    /// <param name="stateComponent">State component.</param>
    private void SetSelection(StateComponent stateComponent)
    {
      theSprite.Alpha = stateComponent.IsSelected ? 0.3f : 1f;
    }
  }
}
