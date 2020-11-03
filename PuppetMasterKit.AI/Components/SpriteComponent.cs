using System;
using System.Diagnostics;
using LightInject;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;

namespace PuppetMasterKit.AI.Components
{
  public class SpriteComponent : Component, IAgentDelegate
  {
    private ISprite theSprite;

    private string currentOrientation = Orientation.S;

    private string currentState = null;

    private string atlasName = null;

    private Size size = Size.Zero;

    private Point anchorPoint;

    public const string ENTITY_ID_PPROPERTY = "id";

    ICoordinateMapper mapper;

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
    public SpriteComponent(string atlasName, Size size = null, Point anchorPoint = null)
    {
      this.atlasName = atlasName;
      this.size = size;
      this.anchorPoint = anchorPoint ?? new Point(0.5f, 0.5f);//default anchor point is the middle of the sprite
      mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
    }

    /// <summary>
    /// On set entity
    /// </summary>
    public override void OnSetEntity()
    {
      var state = Entity.GetComponent<StateComponent>();
      if (state == null)
        throw new Exception("The state component has to be created before the sprite component");

      currentState = state.ToString();
      theSprite = GetSprite(atlasName, currentOrientation, currentState);
      if (theSprite == null)
        throw new Exception("The sprite could not be created");

      theSprite.AddProperty(ENTITY_ID_PPROPERTY, Entity.Id);
      theSprite.AddToScene();
      if (size != null) {
        theSprite.Size = size;
      }

      var agent = Entity.GetComponent<Agent>();
      if (agent != null) {
        theSprite.Position = agent.Position;
      }
      base.OnSetEntity();
    }

    /// <summary>
    /// Gets the name of the texture.
    /// </summary>
    /// <returns>The texture name.</returns>
    /// <param name="atlas">Atlas.</param>
    /// <param name="orientation">Orientation.</param>
    /// <param name="state">State.</param>
    private String GetTextureName(String atlas, string orientation,
                              string state)
    {
      orientation = mapper.ToSceneOrientation(orientation);
      var strState = String.IsNullOrEmpty(state) ? "" : $"-{state}";
      var strOrientation = String.IsNullOrEmpty(orientation) ? "" : $"-{orientation}";
      var suffix = $"{atlas}/{atlas}{strState}{strOrientation}.atlas";
      return suffix;
    }
    /// <summary>
    /// Gets the sprite.
    /// </summary>
    /// <returns>The sprite.</returns>
    /// <param name="atlas">Atlas.</param>
    /// <param name="orientation">Orientation.</param>
    /// <param name="state">State.</param>
    private ISprite GetSprite(String atlas, string orientation, string state)
    {
      var factory = Container.GetContainer().GetInstance<ISpriteFactory>();
      var texture = GetTextureName(atlas, orientation, state);
      return factory.FromTexture(texture);
    }

    /// <summary>
    /// Changes the texture.
    /// </summary>
    /// <returns>The texture.</returns>
    /// <param name="atlas">Atlas.</param>
    /// <param name="orientation">Orientation.</param>
    /// <param name="state">State.</param>
    private ISprite ChangeTexture(String atlas, string orientation, string state, float fps)
    {
      var factory = Container.GetContainer().GetInstance<ISpriteFactory>();
      var texture = GetTextureName(atlas, orientation, state);
      var speed = 1 / fps;
      var sprite = factory.ChangeTexture(theSprite, texture, speed);
      if (sprite != null) {
        sprite.AnchorPoint = anchorPoint;
      }
      return sprite;
    }

    /// <summary>
    /// Agent will update.
    /// </summary>
    /// <param name="agent">Agent.</param>
    public void AgentWillUpdate(Agent agent)
    {
      agent.Position.X = theSprite.Position.X;
      agent.Position.Y = theSprite.Position.Y;
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
        var state = Entity.GetComponent<StateComponent>();
        PhysicsComponent physics = Entity.GetComponent<PhysicsComponent>();
        
        if (direction == Vector.Zero && currentState == state.ToString())
          return;

        if (orientation != currentOrientation || currentState != state.ToString()) {
          currentState = state.ToString();
          if (orientation != null) {
            currentOrientation = orientation;
          }

          var maxSpeed = physics?.MaxSpeed ?? 1;
          var currentSpeed = agent.Velocity.Magnitude() > 0 ? Math.Min(maxSpeed, agent.Velocity.Magnitude()) : maxSpeed;
          var fps = currentSpeed * 24 / maxSpeed;

          var newTexture = ChangeTexture(atlasName, currentOrientation, currentState, fps);
          if (newTexture != null) {
            theSprite = newTexture;
          }
          SetSelection(state);
        }
      }
      theSprite.Position = agent.Position;
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();
      var x = agent.Position.X;
      var y = agent.Position.Y;
      var mx = flightMap.MapWidth;
      var my = flightMap.MapHeight;
      var d = (float)Math.Sqrt(x * x + y * y);
      var D = (float)Math.Sqrt(mx * mx + my * my);
      theSprite.ZOrder = d / D;
    }

    /// <summary>
    /// Sets the selection.
    /// </summary>
    /// <param name="stateComponent">State component.</param>
    private void SetSelection(StateComponent stateComponent)
    {
      //theSprite.Alpha = stateComponent.IsSelected ? 0.7f : 1f;
    }
  }
}
