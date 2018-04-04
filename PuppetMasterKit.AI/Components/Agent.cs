using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Utility;
using System;

namespace PuppetMasterKit.AI.Components
{
  public class Agent : Component
  {
    public Point Position { get; set; }

    public Vector Velocity { get; private set; }

    List<Tuple<Goal, float>> goals = new List<Tuple<Goal, float>>();

    List<Constraint> constraints = new List<Constraint>();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Components.Agent"/> class.
    /// </summary>
    public Agent()
    {
      Position = Point.Zero;
      Velocity = Vector.Zero;
    }

    /// <summary>
    /// Add the specified goal and weight.
    /// </summary>
    /// <returns>The add.</returns>
    /// <param name="goal">Goal.</param>
    /// <param name="weight">Weight.</param>
    public Agent Add(Goal goal, float weight = 1)
    {
      goals.Add(new Tuple<Goal, float>(goal, weight));
      return this;
    }

    /// <summary>
    /// Add the specified constraint.
    /// </summary>
    /// <returns>The add.</returns>
    /// <param name="constraint">Constraint.</param>
    public Agent Add(Constraint constraint)
    {
      constraints.Add(constraint);
      return this;
    }

    /// <summary>
    /// Remove this instance.
    /// </summary>
    /// <returns>The remove.</returns>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public Agent Remove<T>() where T : Goal
    {
      goals.RemoveAll(x => x.Item1 is T);
      return this;
    }

    /// <summary>
    /// Stop the agent by clearing the velocity vector
    /// </summary>
    public void Stop()
    {
      Velocity = Vector.Zero;
    }

    /// <summary>
    /// Calculates the resultant force of the goals
    /// </summary>
    /// <returns>The force.</returns>
    public Vector Force()
    {
      var resultant = Vector.Zero;

      foreach (var goal in goals) {
        resultant += goal.Item1.Force(this) * goal.Item2;
      }

      var maxSpeed = Entity.GetComponent<PhysicsComponent>()?.MaxSpeed ?? 1;

      resultant = resultant.Truncate(maxSpeed);

      foreach (var item in constraints) {
        resultant = item.Constrain(this, resultant);
      }

      return resultant;
    }

    /// <summary>
    /// Update the specified deltaTime.
    /// </summary>
    /// <returns>The update.</returns>
    /// <param name="deltaTime">Delta time.</param>
    public override void Update(double deltaTime)
    {
      var delegates = Entity.GetComponents<IAgentDelegate>();

      delegates.ForEach(x => x.AgentWillUpdate(this));

      if (Position != null) {
        Velocity = Force();
        Position.X += Velocity.Dx;
        Position.Y += Velocity.Dy;
      }

      delegates.ForEach(x => x.AgentDidUpdate(this));
    }

    /// <summary>
    /// Cleanup this instance.
    /// </summary>
    public override void Cleanup()
    {
      base.Cleanup();
    }
  }
}
