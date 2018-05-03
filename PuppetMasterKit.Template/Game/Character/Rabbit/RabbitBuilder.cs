using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;
using LightInject;
using PuppetMasterKit.AI.Rules;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public static class RabbitBuilder
  {
    private static string CharacterName = "rabbit";
    /// <summary>
    /// Build the specified componentSystem and flightMap.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="componentSystem">Component system.</param>
    public static Entity Build(ComponentSystem componentSystem, Polygon boundaries)
    {
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();

      var entity = EntityBuilder.Build()
        .With(componentSystem,
              //new RuleSystemComponent<FlightMap, RabbitHandlers>(
              //  RabbitRulesBuilder.Build(flightMap), new RabbitHandlers()),
              new StateComponent<RabbitStates>(RabbitStates.idle),
              new SpriteComponent(CharacterName, new Size(30, 30)),
              new HealthComponent(100, 20, 3),
              new PhysicsComponent(5, 10, 1, 30),
              new CommandComponent(RabbitHandlers.OnTouched, RabbitHandlers.OnMoveToPoint),
              new CollisionComponent((e) => flightMap.GetAdjacentEntities(e, p => p.Name == "store"), RabbitHandlers.GatherFood, 35),
              new Agent())
        .WithName(CharacterName)
        .GetEntity();
      
      entity.GetComponent<Agent>()
          .Add(new GoalToCohereWith(x => flightMap.GetAdjacentEntities(entity, p => p.Name == CharacterName), 150), 0.001f)
          .Add(new GoalToSeparateFrom( x => flightMap.GetAdjacentEntities(entity, p => p.Name == CharacterName), 50), 0.005f)
          .Add(new ConstraintToStayWithin(boundaries))
          .Add(new GoalToAvoidObstacles(x => ((GameFlightMap)flightMap).GetObstacles(entity), 30));
      
      return entity;
    }
  }
}
