﻿using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Utility.Extensions;

namespace PuppetMasterKit.AI.Goals
{
  public class GoalToCohereWith : Goal
  {
    private Func<Entity, IEnumerable<Entity>> entitiesProvider;

    private float maxDistance;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Goals.GoalToCohereWith"/> class.
    /// </summary>
    /// <param name="entitiesProvider">Entities.</param>
    /// <param name="maxDistance">Max distance.</param>
    public GoalToCohereWith(Func<Entity, IEnumerable<Entity>> entitiesProvider, float maxDistance = 10)
    {
      this.entitiesProvider = entitiesProvider;
      this.maxDistance = maxDistance;
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
                 && Point.Distance(agent.Position, a.Position) < maxDistance);

      if (agent.Position == null || otherAgents.Count() == 0)
        return Vector.Zero;

      var center = Vector.Zero;

      otherAgents.ForEach(x => center += x.Position);

      center = center / otherAgents.Count();

      var coherence = (center - agent.Position);

      return coherence;
    }
  }
}
