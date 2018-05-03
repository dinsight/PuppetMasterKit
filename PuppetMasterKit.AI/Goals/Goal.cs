using System;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Goals
{
  public class Goal
  {
    /// <summary>
    /// Cacluated the force the specified agent.
    /// </summary>
    /// <returns>The force.</returns>
    /// <param name="agent">Agent.</param>
    public virtual Vector Force(Agent agent)
    {
      return Vector.Zero;
    }

    /// <summary>
    /// Calculates the agent's steering force towards a target
    /// </summary>
    /// <returns>The steering.</returns>
    /// <param name="agent">Agent.</param>
    /// <param name="target">Target.</param>
    public Vector Steering(Agent agent, Point target)
    {
      var physiscs = agent.Entity.GetComponent<PhysicsComponent>();
      var maxSpeed = physiscs?.MaxSpeed ?? 1;
      var mass = physiscs?.Mass ?? 1;
      var direction = new Vector(target.X - agent.Position.X,
                                 target.Y - agent.Position.Y);
      var desiredVelocity = direction.Unit() * maxSpeed;
      var steering = (desiredVelocity - agent.Velocity) / mass;
      return steering;
    }
  }
}
