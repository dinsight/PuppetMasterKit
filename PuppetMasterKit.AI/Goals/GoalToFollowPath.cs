using System;
using System.Collections.Generic;
using System.Diagnostics;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Goals
{
  public class GoalToFollowPath : Goal
  {
    private Action<Agent, Point> arrivedHandler;

    private Point[] path;

    private float pathNodeRadius;

    private int currentPointIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Goals.GoalToFollowPath"/> class.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <param name="pathNodeRadius">Path node radius.</param>
    public GoalToFollowPath(Point[] path, float pathNodeRadius = 10)
    {
      Debug.Assert(pathNodeRadius > 0, "pathNodeRadius must be greater than zero");
      this.path = path;
      this.pathNodeRadius = pathNodeRadius;
      this.currentPointIndex = 0;
    }

    /// <summary>
    /// Set up a handler to be callend when the agent arrives at destination
    /// </summary>
    /// <returns>The arrived.</returns>
    /// <param name="arrivedHandler">Arrived handler.</param>
    public GoalToFollowPath WhenArrived(Action<Agent, Point> arrivedHandler)
    {
      this.arrivedHandler = arrivedHandler;
      return this;
    }

    /// <summary>
    /// Force the specified agent.
    /// </summary>
    /// <returns>The force.</returns>
    /// <param name="agent">Agent.</param>
    public override Vector Force(Agent agent)
    {
      if (path.Length <= 1 || agent.Position == null)
        return Vector.Zero;
      
      if (currentPointIndex >= path.Length) {
        TriggerArrivedEvent(agent);
        return Vector.Zero;
      }

      var target = path[currentPointIndex];
      var velocity = agent.Velocity + Steering(agent, target);
      var dist = (velocity + agent.Position) - target;

      if (currentPointIndex == path.Length - 1) {
        //slow down as we approach the last node
        var lastDist = Point.Distance(target, path[currentPointIndex - 1]);
          //velocity *= dist.Magnitude() / lastDist;
      }

      //When we get close enough to this point, vector in on the next one
      if (dist.Magnitude() < pathNodeRadius) {
        currentPointIndex += 1;
      }
      return velocity;
    }

    /// <summary>
    /// Triggers the event and disarms the handler
    /// </summary>
    /// <param name="agent">Agent.</param>
    private void TriggerArrivedEvent(Agent agent)
    {
      if (arrivedHandler != null) {
        arrivedHandler(agent, path[path.Length - 1]);
        arrivedHandler = null;
      }
      agent.Remove<GoalToFollowPath>();
    }
  }
}
