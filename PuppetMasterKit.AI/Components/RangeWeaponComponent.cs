using System;
using System.Collections.Generic;
using System.Linq;
using LightInject;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Utility.Configuration;

namespace PuppetMasterKit.AI.Components
{
  public class RangeWeaponComponent : Component
  {
    private ISprite weaponSprite;
    private Size size;
    private Point currentPosition;
    private Point target;
    private Vector directionUnit;
    private string atlasName;
    private string spriteName;
    private readonly float range;
    private float damagePoints;
    private readonly float velocity;
    private float distanceToTarget;
    private float distanceTillNow;

    private Func<Entity, IEnumerable<Entity>> entitiesProvider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="atlasName"></param>
    /// <param name="spriteName"></param>
    /// <param name="size"></param>
    /// <param name="range"></param>
    /// <param name="damagePoints"></param>
    /// <param name="velocity"></param>
    public RangeWeaponComponent(
      Func<Entity, IEnumerable<Entity>> entitiesProvider,
      string atlasName,
      string spriteName,
      Size size,
      float range,
      float damagePoints,
      float velocity)
    {
      this.size = size;
      this.range = range;
      this.damagePoints = damagePoints;
      this.atlasName = atlasName;
      this.spriteName = spriteName;
      this.velocity = velocity;
      this.entitiesProvider = entitiesProvider;
      distanceToTarget = 0;
      distanceTillNow = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="atPoint"></param>
    public void Fire(Point atPoint)
    {
      if (weaponSprite != null) {
        weaponSprite.RemoveFromParent();
        weaponSprite = null;
      }

      var agent = Entity.GetComponent<Agent>();
      var factory = Container.GetContainer().GetInstance<ISpriteFactory>();
      weaponSprite = factory.CreateFromTexture(atlasName, spriteName);
      weaponSprite.AddToScene();
      weaponSprite.Position = agent.Position;
      weaponSprite.Size = size;

      this.target = atPoint.Clone();
      this.currentPosition = agent.Position.Clone();
      //this.currentPosition.X -= 2*size.Width;
      //this.currentPosition.Y -= 2*size.Height;
      var direction = (target - agent.Position);
      directionUnit = direction.Unit();
      this.distanceToTarget = Math.Min( direction.Magnitude(), this.range);
      this.distanceTillNow = 0;
      //change the agent's position a little so when the direction of the sprite
      //is calculated, it should face the direction of the missile being thrown
      agent.Position = (agent.Position + directionUnit).ToPosition(); 
    }

    /// <summary>
    /// 
    /// </summary>
    public override void OnSetEntity()
    {
      base.OnSetEntity();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="deltaTime"></param>
    public override void Update(double deltaTime)
    {
      if (weaponSprite != null)
      {
        var displacementInterval = (float)(this.velocity * deltaTime);
        currentPosition += (this.directionUnit * displacementInterval).ToPosition();
        distanceTillNow += displacementInterval;
        weaponSprite.Position = currentPosition;

        if (distanceToTarget - distanceTillNow <= 0.5 ) {
          weaponSprite.RemoveFromParent();
          weaponSprite = null;
          currentPosition = target;
          DoDamageTargets();
        } 
      }
      base.Update(deltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
    private void DoDamageTargets()
    {
      var thisAgent = Entity.GetComponent<Agent>();
      var inRange = entitiesProvider(Entity)
        .Where(x => {
        var agent = x.GetComponent<Agent>();
        return agent!=null && Point.Distance(thisAgent.Position, agent.Position) <= this.range;
      });
      foreach (var entity in inRange) {
        var health = entity.GetComponent<HealthComponent>();
        health.Damage += this.damagePoints;
        if (health.Damage >= health.MaxHealth) {
          entity.Dispose();
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Cleanup()
    {
      base.Cleanup();
    }
  }
}
