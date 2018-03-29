using System;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Goals
{
    public class Constraint
    {
        public Constraint()
        {
        }

        /// <summary>
        /// Constrain the specified agent and resultant.
        /// </summary>
        /// <returns>The constrain.</returns>
        /// <param name="agent">Agent.</param>
        /// <param name="resultant">Resultant.</param>
        public Vector Constrain(Agent agent, Vector resultant) 
        {
            return Vector.Zero;
        }
    }
}
