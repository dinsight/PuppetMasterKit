using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Utility.Extensions;

namespace PuppetMasterKit.AI.Goals
{
  public class GoalToEvade : Goal
  {
    private Func<Entity, IEnumerable<Entity>> entitiesProvider;

    private float minRadius;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Goals.GoalToEvade"/> class.
    /// </summary>
    /// <param name="entitiesProvider">Entities.</param>
    /// <param name="minRadius">Minimum radius.</param>
    public GoalToEvade(Func<Entity, IEnumerable<Entity>> entitiesProvider, float minRadius = 100)
    {
      this.entitiesProvider = entitiesProvider;
      this.minRadius = minRadius;
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
      var otherAgents = entitiesProvider(agent.Entity)
          .Select(x => x.GetComponent<Agent>())
          .Where(a => a != null
                 && !Object.ReferenceEquals(agent, a)
                 && a.Position != null
                 && Point.Distance(agent.Position, a.Position) < minRadius);

      if (agent.Position == null || otherAgents.Count() == 0)
        return Vector.Zero;

      var flee = Vector.Zero;

      otherAgents.ForEach(x => {
        var dist = Point.Distance(agent.Position, x.Position);
        var maxVelocity = x.Entity.
                        GetComponent<PhysicsComponent>()?.MaxSpeed ?? 1;
        var lookAhead = dist / maxVelocity;
        //calculate a point ahead the agent to follow
        var ahead = x.Position + x.Velocity.Unit() * lookAhead;
        var target = new Point(ahead.Dx, ahead.Dy);

        flee = flee - Steering(agent, target);
      });

      return flee;
    }
  }
}
