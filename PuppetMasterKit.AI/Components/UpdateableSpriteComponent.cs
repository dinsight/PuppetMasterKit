using System;
using LightInject;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Utility.Configuration;

namespace PuppetMasterKit.AI.Components
{
  public class UpdateableSpriteComponent : SpriteComponent, IAgentDelegate
  {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="atlasName"></param>
    /// <param name="size"></param>
    /// <param name="anchorPoint"></param>
    /// <param name="initialOrientation"></param>
    public UpdateableSpriteComponent(string atlasName,
      Size size = null,
      Point anchorPoint = null,
      Orientation? initialOrientation = Orientation.S) : base(atlasName, size, anchorPoint)
    {
      this.CurrentOrientation = initialOrientation;
    }

    /// <summary>
    /// Agent will update.
    /// </summary>
    /// <param name="agent">Agent.</param>
    public void AgentWillUpdate(Agent agent)
    {
      if (Sprite.Position != null) {
        agent.Position.X = Sprite.Position.X;
        agent.Position.Y = Sprite.Position.Y;
      }
    }

    /// <summary>
    /// Agent did update.
    /// </summary>
    /// <param name="agent">Agent.</param>
    public void AgentDidUpdate(Agent agent)
    {
      var fps = GetFps(Entity);
      if (Sprite.Position != null) {
        var direction = agent.Position - Sprite.Position;
        var orientation = OrientationExtension.GetOrientation(direction);
        var state = Entity.GetComponent<StateComponent>();

        if (direction == Vector.Zero && CurrentState == state.ToString())
          return;

        if (orientation != CurrentOrientation || CurrentState != state.ToString()) {
          CurrentState = state.ToString();
          if (orientation != null) {
            CurrentOrientation = orientation;
          }
          var newTexture = ChangeTexture(AtlasName, CurrentOrientation, CurrentState, fps);
          if (newTexture != null) {
            Sprite = newTexture;
          }
          SetSelection(state);
        }
      } else {
        var newTexture = ChangeTexture(AtlasName, CurrentOrientation, CurrentState, fps);
        if (newTexture != null) {
          Sprite = newTexture;
        }
      }
      UpdatePositionFromAgent(agent);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    private void SetSelection(StateComponent state)
    {
      
    }
  }
}
