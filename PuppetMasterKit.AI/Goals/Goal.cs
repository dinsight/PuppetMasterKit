using System;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Goals
{
    public class Goal
    {
        public Goal()
        {
        }

        /// <summary>
        /// Cacluated the force the specified agent.
        /// </summary>
        /// <returns>The force.</returns>
        /// <param name="agent">Agent.</param>
        public virtual Vector Force(Agent agent)
        {
            return Vector.Zero;
        }
    }
}
