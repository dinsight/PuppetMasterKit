using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public static class RabbitBuilder
  {
    /// <summary>
    /// Build the specified componentSystem and flightMap.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="componentSystem">Component system.</param>
    /// <param name="flightMap">Flight map.</param>
    public static Entity Build(ComponentSystem componentSystem, 
                               FlightMap flightMap)
    {
      var entity = EntityBuilder.Build()
        .With(componentSystem,
          new StateComponent<RabbitStates>(RabbitStates.idle),
          new SpriteComponent("rabbit", new Size(30, 30)),
          new PhysicsComponent(10, 5, 1, 5),
          new CommandComponent(RabbitHandlers.OnTouched,
                               RabbitHandlers.OnMoveToPoint),
          new Agent())
        .WithName("rabbit")
        .GetEntity();

      entity.GetComponent<Agent>()
            .Add(new GoalToCohereWith(x=>flightMap.GetEntities(p=>p.Name == "rabbit"), 150), 0.001f)
            .Add(new GoalToSeparateFrom(x => flightMap.GetEntities(p => p.Name == "rabbit"), 50), 0.002f);

      flightMap.Add(entity);
      return entity;
    }
  }
}
