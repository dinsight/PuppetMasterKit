using System.Linq;
using LightInject;
using System.Collections.Generic;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Utility;
using System;
using PuppetMasterKit.AI.Configuration;

namespace PuppetMasterKit.AI.Components
{
  public class Agent : Component
  {
    private Point position = Point.Zero;

    private List<Tuple<Goal, float>> goals = new List<Tuple<Goal, float>>();

    private List<Constraint> constraints = new List<Constraint>();

    public Vector Velocity { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Components.Agent"/> class.
    /// </summary>
    public Agent()
    {
      position = Point.Zero;
      Velocity = Vector.Zero;
    }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>The position.</value>
    public Point Position {
      get { return position; }
      set {
        position = value;
        if (Entity != null) {
          var flightMap = Container.GetContainer().GetInstance<FlightMap>();
          var delegates = Entity.GetComponents<IAgentDelegate>();
          delegates.ForEach(x => x.AgentDidUpdate(this));
          flightMap.AgentDidUpdate(this);
        }
      }
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
      var maxSpeed = Entity.GetComponent<PhysicsComponent>()?.MaxSpeed ?? 1;

      foreach (var goal in goals.ToArray()) {
        resultant = resultant + goal.Item1.Force(this) * goal.Item2;
      }

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
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();
      var delegates = Entity.GetComponents<IAgentDelegate>();

      delegates.ForEach(x => x.AgentWillUpdate(this));
      flightMap.AgentWillUpdate(this);

      if (Position != null) {
        Velocity = Force();
        position = new Point(Velocity.Dx + position.X, Velocity.Dy + position.Y);
      }

      delegates.ForEach(x => x.AgentDidUpdate(this));
      flightMap.AgentDidUpdate(this);
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
