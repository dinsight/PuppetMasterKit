using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Goals
{
    public class GoalToAvoidObstacles : Goal
    {
        private Func<List<Polygon>> obstacles;

        private float maxSeeAhead;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Goals.GoalToAvoidObstacles"/> class.
        /// </summary>
        /// <param name="obstacles">Obstacles.</param>
        /// <param name="maxSeeAhead">Max see ahead.</param>
        public GoalToAvoidObstacles(Func<List<Polygon>> obstacles, float maxSeeAhead = 20)
        {
            this.obstacles = obstacles;
            this.maxSeeAhead = maxSeeAhead;
        }

        /// <summary>
        /// Force the specified agent.
        /// </summary>
        /// <returns>The force.</returns>
        /// <param name="agent">Agent.</param>
        public override Vector Force(Agent agent)
        {
            if (obstacles().Count == 0 || agent.Position == null)
                return Vector.Zero;

            var avoidance = Vector.Zero;
            var probe = (agent.Velocity.Unit() * maxSeeAhead
                         + agent.Position).ToPosition();

            var obstacleHit = obstacles()
                .FirstOrDefault(x => x.IsPointInside(probe));

            if (obstacleHit!=null){
                avoidance = (probe - obstacleHit.Centroid()).Unit() 
                            * agent.Velocity.Magnitude();
            }

            return avoidance;
        }
    }
}
