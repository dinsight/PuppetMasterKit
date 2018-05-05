using System;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Goals
{
  public class ConstraintToStayWithin : Constraint
  {
    Polygon limits;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Goals.ConstraintToStayWithin"/> class.
    /// </summary>
    /// <param name="limits">Limits.</param>
    public ConstraintToStayWithin(Polygon limits)
    {
      this.limits = limits;
    }

    /// <summary>
    /// Constrain the specified agent and resultant.
    /// </summary>
    /// <returns>The constrain.</returns>
    /// <param name="agent">Agent.</param>
    /// <param name="resultant">Resultant.</param>
    public override Vector Constrain(Agent agent, Vector resultant)
    {
      if (agent.Position == null)
        return Vector.Zero;

      var probe = (agent.Position + resultant).ToPosition();
      if (!limits.IsPointInside(probe)) {
        return Vector.Zero;
      }

      return resultant;
    }
  }
}
