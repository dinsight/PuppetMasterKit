using System;
using System.Collections.Generic;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Goals
{
    public class GoalToFollowAgent : Goal
    {
        private Func<Agent> agentToFollow;

        private float behindDistance;

        private float slowingRadius;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Goals.GoalToFollowAgent"/> class.
        /// </summary>
        /// <param name="agentToFollow">Agent to follow.</param>
        /// <param name="behindDistance">Behind distance.</param>
        /// <param name="slowingRadius">Slowing radius.</param>
        public GoalToFollowAgent(Func<Agent> agentToFollow, 
                                 float behindDistance = 5,
                                 float slowingRadius = 1)
        {
            this.agentToFollow = agentToFollow;
            this.behindDistance = behindDistance;
            this.slowingRadius = slowingRadius;
        }

        /// <summary>
        /// Force the specified agent.
        /// </summary>
        /// 
        /// https://gamedevelopment.tutsplus.com/tutorials/understanding-steering-behaviors-leader-following--gamedev-10810
        /// 
        /// <returns>The force.</returns>
        /// <param name="agent">Agent.</param>
		public override Vector Force(Agent agent)
		{
            if (agentToFollow() == null || agent.Position == null)
                return Vector.Zero;

            var behind = Vector.Zero;

            if(agentToFollow().Velocity == Vector.Zero){
                behind = agentToFollow().Position 
                    - (agentToFollow().Position - agent.Position).Unit() 
                    * behindDistance;
            } else {
                behind = agentToFollow().Position 
                    - agentToFollow().Velocity.Unit() * behindDistance; 
            }

            var target = new Point(behind.Dx, behind.Dy);
            //calc the velocity from this point to the point behind the agent 
            //to follow
            var velocity = agent.Velocity - Steering(agent, target);
            var dist = Point.Distance(agent.Position, target);
            //if we're getting close to the target, slow down
            if (dist<slowingRadius){
                velocity = velocity * (dist / slowingRadius);
            }
            return velocity;
		}
	}
}
