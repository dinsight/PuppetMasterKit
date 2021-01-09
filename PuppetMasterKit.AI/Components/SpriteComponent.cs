using System;
using System.Diagnostics;
using LightInject;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Utility.Attributes;

namespace PuppetMasterKit.AI.Components
{
  public class SpriteComponent : Component
  {
    public const string ENTITY_ID_PPROPERTY = "id";
    private ISprite theSprite;
    private string currentState = null;
    private Orientation? currentOrientation = null;
    private string atlasName = null;
    private Size size = Size.Zero;
    private Point anchorPoint;
    private ICoordinateMapper mapper;

    #region Properties
    public ISprite Sprite { get => theSprite; protected set => theSprite = value; }

    public string CurrentState { get => currentState; protected set => currentState = value; }

    public string AtlasName { get => atlasName; private set => atlasName = value; }

    public Orientation? CurrentOrientation { get => currentOrientation; protected set => currentOrientation = value; }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="atlasName"></param>
    /// <param name="size"></param>
    /// <param name="anchorPoint"></param>
    /// <param name="initialOrientation"></param>
    public SpriteComponent(string atlasName,
      Size size = null,
      Point anchorPoint = null,
      Orientation? initialOrientation = Orientation.S)
    {
      this.atlasName = atlasName;
      this.size = size;
      //default anchor point is the middle of the sprite
      this.anchorPoint = anchorPoint ?? new Point(0.5f, 0.5f);
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

      CurrentState = state.ToString();
      if (Sprite == null) {
        Sprite = CreateSprite(atlasName, CurrentOrientation, CurrentState);
        if (Sprite == null)
          throw new Exception("The sprite could not be created");

        Sprite.AddProperty(ENTITY_ID_PPROPERTY, Entity.Id);
        Sprite.AddToScene();
      } else {
        ChangeTexture(AtlasName, CurrentOrientation, CurrentState, GetFps(Entity));
      }
      
      if (size != null) {
        Sprite.Size = size;
      }

      var agent = Entity.GetComponent<Agent>();
      if (agent != null) {
        UpdatePositionFromAgent(agent);
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
    private String GetTextureName(String atlas, Orientation? orientation,
                              string state)
    {
      orientation = mapper.ToSceneOrientation(orientation);
      var strState = String.IsNullOrEmpty(state) ? "" : $"-{state}";
      var strOrientation = orientation.HasValue ? $"-{orientation.GetStringValue()}" : "";
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
    private ISprite CreateSprite(String atlas, Orientation? orientation, string state)
    {
      var factory = Container.GetContainer().GetInstance<ISpriteFactory>();
      var texture = GetTextureName(atlas, orientation, state);
      return factory.CreateFromTexture(texture);
    }

    /// <summary>
    /// Changes the texture.
    /// </summary>
    /// <returns>The texture.</returns>
    /// <param name="atlas">Atlas.</param>
    /// <param name="orientation">Orientation.</param>
    /// <param name="state">State.</param>
    protected ISprite ChangeTexture(String atlas, Orientation? orientation, string state, float fps)
    {
      var factory = Container.GetContainer().GetInstance<ISpriteFactory>();
      var texture = GetTextureName(atlas, orientation, state);
      var speed = 1 / fps;
      var sprite = factory.ChangeTexture(Sprite, texture, speed);
      if (sprite != null) {
        sprite.AnchorPoint = anchorPoint;
        if (size != null) {
          sprite.Size = size;
        }
      }
      return sprite;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected float GetFps(Entity entity)
    {
      PhysicsComponent physics = entity.GetComponent<PhysicsComponent>();
      var maxSpeed = physics?.MaxSpeed ?? 1;
      //var currentSpeed = agent.Velocity.Magnitude() > 0 ? Math.Min(maxSpeed, agent.Velocity.Magnitude()) : maxSpeed;
      var currentSpeed = maxSpeed;
      var fps = currentSpeed * 24 / maxSpeed;
      return fps;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="agent"></param>
    protected void UpdatePositionFromAgent(Agent agent)
    {
      Sprite.Position = agent.Position;
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();
      var x = agent.Position.X;
      var y = agent.Position.Y;
      var mx = flightMap.MapWidth;
      var my = flightMap.MapHeight;
      var d = (float)Math.Sqrt(x * x + y * y);
      var D = (float)Math.Sqrt(mx * mx + my * my);
      Sprite.ZOrder = d / D;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Cleanup()
    {
      Sprite.RemoveFromParent();
      base.Cleanup();
    }
  }
}
