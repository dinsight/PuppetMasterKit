using System;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.Template.Game.Character
{
  public class RabbitBuilder
  {
    /// <summary>
    /// Build the specified componentSystem.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="componentSystem">Component system.</param>
    public static Entity Build(ComponentSystem componentSystem)
    {
      var entity = EntityBuilder.Build()
        .With(componentSystem,
          new StateComponent<RabbitStates>(RabbitStates.idle),
          new SpriteComponent("rabbit", new Size(30, 30)),
          new PhysicsComponent(35, 2, 1, 5),
          new CommandComponent(OnTouched, OnMoveToPoint),
          new Agent())
        .WithName("rabbit")
        .GetEntity();
      
      return entity;
    }

    /// <summary>
    /// Ons the touched.
    /// </summary>
    /// <param name="rabbit">Rabbit.</param>
    private static void OnTouched(Entity rabbit)
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
    private static void OnMoveToPoint(Entity entity, Point location)
    {
      var agent = entity.GetComponent<Agent>();
      if (agent == null)
        return;
      
      //remove existing follow path command
      agent.Remove<GoalToFollowPath>();
      //create new goal. Makes sure the goal is deleted upon arrival
      var goToPoint = new GoalToFollowPath(new Point[] { agent.Position, location })
        //.WhenArrived((x,p)=> x.Remove<GoalToFollowPath>())
        ;
      
      agent.Add(goToPoint, 3);
    }
  }
}
