using System;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Goals
{
  public class GoalToWander : Goal
  {
    private float circleRadius;

    private float circleDistance;

    private int maxChangeAngle;

    private float angleChange;

    private float wanderAngle;

    private int iteration;

    private int changeAfterIteration;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Goals.GoalToWander"/> class.
    /// </summary>
    /// <param name="circleRadius">The radius around the agent where we apply the wander force.</param>
    /// <param name="circleDistance">The distance from the agent to the center of the circle.</param>
    /// <param name="maxChangeAngle">The max angle change .</param>
    public GoalToWander(float circleRadius = 7,
                        float circleDistance = 20,
                        int maxChangeAngle = 10,
                        int changeAfterIteration=500)
    {
      this.circleRadius = circleRadius;
      this.circleDistance = circleDistance;
      this.maxChangeAngle = maxChangeAngle;
      this.angleChange = 0.1f;
      this.wanderAngle = 0;
      this.iteration = 0;
      this.changeAfterIteration = changeAfterIteration;
    }

    /// <summary>
    /// Force the specified agent.
    /// </summary>
    /// <returns>The force.</returns>
    /// <param name="agent">Agent.</param>
    public override Vector Force(Agent agent)
    {
      var random = new Random();
      var center = agent.Velocity.Clone().Unit() * circleDistance;
      var displacement = new Vector(0, -1) * circleRadius;
      displacement.SetAngle(wanderAngle);
      if (iteration++ >= changeAfterIteration) {
        wanderAngle = random.Next(maxChangeAngle);
        iteration = 0;
      }
      return center + displacement;
    }
  }
}
