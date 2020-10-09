using System;
namespace PuppetMasterKit.AI.Components
{
  public class PhysicsComponent : Component
  {
    public float Mass { get; private set; }

    public float MaxSpeed { get; private set; }

    public float MaxForce { get; private set; }

    public float Radius { get; private set; }

    public float AttckRange { get; private set; }

    public float Fps { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Components.PhysicsComponent"/> class.
    /// </summary>
    /// <param name="mass">Mass.</param>
    /// <param name="maxSpeed">Max speed.</param>
    /// <param name="maxForce">Max force.</param>
    /// <param name="radius">Radius.</param>
    /// <param name="attackRange">Attack range.</param>
    public PhysicsComponent(float mass = 1, float maxSpeed = 1,
                            float maxForce = 1, float radius = 1,
                            float attackRange = 1, float fps = 24)
    {
      Mass = mass;
      MaxSpeed = maxSpeed;
      MaxForce = maxForce;
      Radius = radius;
      AttckRange = attackRange;
      Fps = fps;
    }
  }
}
