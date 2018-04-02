using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.AI.Goals
{
  public class GoalToFlee : Goal
  {
    private Func<Entity, ICollection<Entity>> entitiesProvider;

    private float minRadius;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Goals.GoalToFlee"/> class.
    /// </summary>
    /// <param name="entitiesProvider">Entities.</param>
    /// <param name="minRadius">Minimum radius.</param>
    public GoalToFlee(Func<Entity, ICollection<Entity>> entitiesProvider, float minRadius = 100)
    {
      this.entitiesProvider = entitiesProvider;
      this.minRadius = minRadius;
    }

    /// <summary>
    /// Force the specified agent.
    /// </summary>
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
        flee = flee - Steering(agent, x.Position);
      });

      return flee;
    }
  }
}
