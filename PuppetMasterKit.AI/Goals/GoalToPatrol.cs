using System;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Goals
{
  public class GoalToPatrol : Goal
  {
    Point[] waypoints;

    private float waypointNodeRadius;

    private int currentPointIndex = 0;

    private int direction = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Goals.GoalToPatrol"/> class.
    /// </summary>
    /// <param name="waypoints">List of points defining the path.</param>
    /// <param name="waypointNodeRadius">The radius of the nodes. When an agent is within
    ///this radius the node's pull force becomes zero.</param>
    public GoalToPatrol(Point[] waypoints, float waypointNodeRadius = 10)
    {
      this.waypointNodeRadius = waypointNodeRadius;
      this.waypoints = waypoints;
      this.currentPointIndex = 0;
    }

    /// <summary>
    /// Force the specified agent.
    /// </summary>
    /// <returns>The force.</returns>
    /// <param name="agent">Agent.</param>
    public override Vector Force(Agent agent)
    {
      if (waypoints.Length == 0 || agent.Position == null)
        return Vector.Zero;

      var target = waypoints[currentPointIndex];
      var velocity = (agent.Velocity + Steering(agent, target));
      //When we get close enough to this point, vector in on the next one
      var dist = velocity + agent.Position - target;

      if (dist.Magnitude() <= waypointNodeRadius) {
        if (currentPointIndex == waypoints.Length - 1 ||
            currentPointIndex == 0) {
          direction *= -1;
        }
        currentPointIndex += direction;
      }

      return velocity;
    }
  }
}
