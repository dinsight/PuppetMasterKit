using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.AI.Goals
{
  public class GoalToSeparateFrom : Goal
  {
    private Func<Entity, IEnumerable<Entity>> entitiesProvider;

    private float keepDistance;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Goals.GoalToSeparateFrom"/> class.
    /// </summary>
    /// <param name="entitiesProvider">Entities.</param>
    /// <param name="keepDistance">Keep distance.</param>
    public GoalToSeparateFrom(Func<Entity, IEnumerable<Entity>> entitiesProvider,
                                  float keepDistance = 10)
    {
      this.entitiesProvider = entitiesProvider;
      this.keepDistance = keepDistance;
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
                 && Point.Distance(agent.Position, a.Position) < keepDistance);

      if (agent.Position == null || otherAgents.Count() == 0)
        return Vector.Zero;

      var separation = Vector.Zero;

      otherAgents.ForEach(x => {
        separation = separation + x.Position - agent.Position;
      });

      separation = separation / otherAgents.Count() * -1;

      return separation;
    }
  }
}
