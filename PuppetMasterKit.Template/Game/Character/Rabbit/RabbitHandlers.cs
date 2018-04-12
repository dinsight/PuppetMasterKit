using System;
using System.Diagnostics;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.AI.Rules;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game.Facts;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public class RabbitHandlers : FactHandler
  {
    /// <summary>
    /// Ons the touched.
    /// </summary>
    /// <param name="rabbit">Rabbit.</param>
    public static void OnTouched(Entity rabbit)
    {
      var state = rabbit.GetComponent<StateComponent>();
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
    public void Handle(Hungry fact)
    {
      Debug.WriteLine("Me Hungry...");
    }
  }
}
