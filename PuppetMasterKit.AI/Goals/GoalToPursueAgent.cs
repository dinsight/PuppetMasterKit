using System;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Goals
{
  public class GoalToPursueAgent : Goal
  {
    private Func<Agent> agentToFollow;

    /// <summary>
    /// The Agent to follow
    /// </summary>
    /// <param name="agentToFollow">Agent to follow.</param>
    public GoalToPursueAgent(Func<Agent> agentToFollow)
    {
      this.agentToFollow = agentToFollow;
    }

    /// <summary>
    /// Force the specified agent.
    /// </summary>
    /// https://gamedevelopment.tutsplus.com/tutorials/understanding-steering-behaviors-pursuit-and-evade--gamedev-2946
    /// 
    /// <returns>The force.</returns>
    /// <param name="agent">Agent.</param>
    public override Vector Force(Agent agent)
    {
      if (agent.Position == null || agentToFollow() == null)
        return Vector.Zero;

      var physiscs = agentToFollow().Entity.GetComponent<PhysicsComponent>();
      var dist = Point.Distance(agent.Position, agentToFollow().Position);
      var maxVelocity = physiscs?.MaxSpeed ?? 1;
      var lookAheadTicks = dist / maxVelocity;

      // no force necessary if we are close to the pursued agent
      if (dist <= physiscs.Radius)
        return Vector.Zero;

      //calculate a point ahead the agent to follow
      var ahead = agentToFollow().Position
          + (agentToFollow().Velocity).Unit() * lookAheadTicks;
      var target = new Point(ahead.Dx, ahead.Dy);

      //calc the velocity from this point to the point behind the agent to follow
      var velocity = agent.Velocity + Steering(agent, target);

      return velocity;
    }
  }
}
