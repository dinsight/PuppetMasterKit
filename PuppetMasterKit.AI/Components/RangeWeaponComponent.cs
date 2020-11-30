using System;
using System.Diagnostics;
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

    private Point current;

    private Point target;

    private Vector directionUnit;

    private float displacementInterval;

    private string atlasName;

    private string spriteName;

    private readonly float range;

    private float damagePoints;

    private readonly float secondsToTarget;

    
    public RangeWeaponComponent(string atlasName,
      string spriteName,
      Size size,
      float range,
      float damagePoints,
      float secondsToTarget)
    {
      this.size = size;
      this.range = range;
      this.damagePoints = damagePoints;
      this.atlasName = atlasName;
      this.spriteName = spriteName;
      this.secondsToTarget = secondsToTarget;
    }

    public void Fire(Point atPoint)
    {
      if (weaponSprite != null) {
        weaponSprite.RemoveFromParent();
        weaponSprite = null;
      }

      var agent = Entity.GetComponent<Agent>();
      var factory = Container.GetContainer().GetInstance<ISpriteFactory>();
      weaponSprite = factory.FromTexture(atlasName, spriteName);
      weaponSprite.AddToScene();
      weaponSprite.Position = agent.Position;
      weaponSprite.Size = size;

      this.target = atPoint;
      this.current = agent.Position;
      var vector = (target - agent.Position);
      this.directionUnit = vector.Unit();
      this.displacementInterval = vector.Magnitude() / 50;
    }

    public override void OnSetEntity()
    {
      base.OnSetEntity();
    }

    public override void Update(double deltaTime)
    {
      if (weaponSprite != null) {
        var nextStep = current + this.directionUnit * displacementInterval;
        weaponSprite.Position = nextStep.ToPosition();
        current = nextStep.ToPosition();
        var dist = Point.Distance(target, current);
        if (dist <= 0.5) {
          weaponSprite.RemoveFromParent();
          weaponSprite = null;
          current = target;
          Debug.WriteLine("Throw done");
        } 
      }
      base.Update(deltaTime);
    }

    public override void Cleanup()
    {
      base.Cleanup();
    }
  }
}
