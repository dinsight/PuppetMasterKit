using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Goals
{
  public class GoalToAvoidObstacles : Goal
  {
    private Func<Entity, IEnumerable<Obstacle>> obstaclesProvider;

    private float maxSeeAhead;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Goals.GoalToAvoidObstacles"/> class.
    /// </summary>
    /// <param name="obstaclesProvider">Obstacles.</param>
    /// <param name="maxSeeAhead">Max see ahead.</param>
    public GoalToAvoidObstacles(Func<Entity, IEnumerable<Obstacle>> obstaclesProvider, 
                                float maxSeeAhead = 20)
    {
      this.obstaclesProvider = obstaclesProvider;
      this.maxSeeAhead = maxSeeAhead;
    }

    /// <summary>
    /// Force the specified agent.
    /// https://gamedevelopment.tutsplus.com/tutorials/understanding-steering-behaviors-collision-avoidance--gamedev-7777
    /// </summary>
    /// <returns>The force.</returns>
    /// <param name="agent">Agent.</param>
    public override Vector Force(Agent agent)
    {
      var obstacles = obstaclesProvider(agent.Entity).ToList();
      if (obstacles.Count == 0 || agent.Position == null)
        return Vector.Zero;

      var maxSpeed = agent.Entity.GetComponent<PhysicsComponent>()?.MaxSpeed ?? 1;
      var lenghtAhead = Math.Max( agent.Velocity.Magnitude() / maxSpeed,  this.maxSeeAhead );
      var probe = (agent.Velocity.Unit() * lenghtAhead + agent.Position).ToPosition();
      var probe2 = (agent.Velocity.Unit() * lenghtAhead * 0.5f + agent.Position).ToPosition();
      var obstacleHit = obstacles.FirstOrDefault(x => x.IsInside(probe) || x.IsInside(probe2));

      var avoidance = Vector.Zero;

      if (!Object.ReferenceEquals(obstacleHit,null)) {
        var centroid = obstacleHit.GetCenterPoint();
        avoidance = (probe - centroid).Truncate(maxSpeed);
      }
      return avoidance;
    }
  }
}
