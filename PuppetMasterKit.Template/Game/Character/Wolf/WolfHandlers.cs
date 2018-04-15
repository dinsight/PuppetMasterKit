using System;
using System.Diagnostics;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.AI.Rules;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game.Facts;

namespace PuppetMasterKit.Template.Game.Character.Wolf
{
  public class WolfHandlers : FactHandler
  {
    /// <summary>
    /// /
    /// </summary>
    /// <param name="wolf">Wolf.</param>
    public static void OnTouched(Entity wolf)
    {
      var state = wolf.GetComponent<StateComponent>();
      if (state != null) {
        state.IsSelected = !state.IsSelected;
      }
    }

    /// <summary>
    /// Ons the scene touched.
    /// </summary>
    /// <param name="location">Location.</param>
    public static void OnMoveToPoint(Entity entity, Point location)
    {
      var agent = entity.GetComponent<Agent>();
      if (agent == null)
        return;

      //remove existing follow path command
      agent.Remove<GoalToFollowPath>();
      //create new goal. Makes sure the goal is deleted upon arrival
      var goToPoint = new GoalToFollowPath(new Point[] { agent.Position, location })
        .WhenArrived((x, p) => { });
      agent.Add(goToPoint, 5);
    }

    /// <summary>
    /// Handle the specified fact.
    /// </summary>
    /// <returns>The handle.</returns>
    /// <param name="fact">Fact.</param>
    public void Handle(Entity target, Hunt fact)
    {
      var agent = target.GetComponent<Agent>();
      var state = target.GetComponent<StateComponent<WolfStates>>();
      if (state.CurrentState != WolfStates.attack) {
        agent.Remove<GoalToFollowAgent>();
        agent.Add(new GoalToPursueAgent(() => fact.GetTarget()?.GetComponent<Agent>()));
        state.CurrentState = WolfStates.attack;
        Debug.WriteLine("Me Hunting...");
      }
    }

    /// <summary>
    /// Wolfs meets the prey.
    /// </summary>
    /// <param name="wolf">Wolf.</param>
    /// <param name="prey">Prey.</param>
    public static void WolfMeetsPrey(Entity wolf, Entity prey)
    {
      
    }
  }
}
